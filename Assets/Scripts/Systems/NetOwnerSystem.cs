// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Jobs;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

public class NetOwnerSystem : JobComponentSystem {
    protected override void OnCreate() {
        Enabled = false;
    }

    private NetworkDriver _driver;
    private NetworkPipeline _pipeline;
    private NetworkConnection _connection;
    private Entity _netEntity;

    protected override void OnStartRunning() {
        if (Entity.Null != _netEntity) {
            return;
        }

        _driver = NetworkDriver.Create(new ReliableUtility.Parameters {
            WindowSize = 32
        });
        _pipeline = _driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        _connection = _driver.Connect(NetworkEndPoint.Parse("127.0.0.1", 8989));
        _netEntity = EntityManager.CreateEntity(typeof(NetOwnerComponent));
    }

    protected override void OnDestroy() {
        if (_driver.IsCreated) {
            _driver.Dispose();
        }
    }

    struct ServerUpdateJob : IJob {
        public NetworkDriver driver;
        public NetworkConnection connection;
        public NetworkPipeline pipeline;

        public void Execute() {
            NetworkEvent.Type cmd;
            while (NetworkEvent.Type.Empty != (cmd = connection.PopEvent(driver, out var stream))) {
                if (cmd == NetworkEvent.Type.Connect) {
                    Debug.Log("We are now connected to the server");

                    uint value = 10;
                    var writer = driver.BeginSend(pipeline, connection);
                    writer.WriteUInt(value);
                    driver.EndSend(writer);
                }
                else if (cmd == NetworkEvent.Type.Data) {
                    Debug.Log($"Got the value = {stream.ReadUInt()} back from the server");
                    connection.Disconnect(driver);
                }
                else if (cmd == NetworkEvent.Type.Disconnect) {
                    Debug.Log("Client got disconnected from server");
                    connection = default;
                }
            }
        }
    }

    private JobHandle _jobHandle;

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        _jobHandle.Complete();

        var updateJob = new ServerUpdateJob {
            driver = _driver,
            connection = _connection,
            pipeline = _pipeline
        };
        _jobHandle = _driver.ScheduleUpdate(inputDeps);
        _jobHandle = updateJob.Schedule(_jobHandle);
        return _jobHandle;
    }
}
