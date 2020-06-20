// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

public class ServerSystem : JobComponentSystem {
    private NetworkDriver _driver;
    private NetworkPipeline _pipeline;
    private NativeList<NetworkConnection> _connections;

    protected override void OnCreate() {
        _driver = NetworkDriver.Create(new ReliableUtility.Parameters {
            WindowSize = 32
        });
        _pipeline = _driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        _connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = 8989;
        if (0 != _driver.Bind(endpoint)) {
            Debug.LogError($"Server endpoint bind failed : Port({endpoint.Port}");
        }
        else {
            _driver.Listen();
        }
    }

    private JobHandle _jobHandle;

    protected override void OnDestroy() {
        _jobHandle.Complete();
        _connections.Dispose();
        _driver.Dispose();
    }

    struct ServerUpdateConnectionsJob : IJob {
        public NetworkDriver driver;
        public NativeList<NetworkConnection> connections;

        public void Execute() {
            for (var i = 0; i < connections.Length; ++i) {
                if (false == connections[i].IsCreated) {
                    connections.RemoveAtSwapBack(i--);
                }
            }

            NetworkConnection c;
            while ((c = driver.Accept()) != default(NetworkConnection)) {
                connections.Add(c);
                Debug.Log("Accepted a connection");
            }
        }
    }

    struct ServerUpdateJob : IJobParallelForDefer {
        public NetworkDriver.Concurrent driver;
        public NativeArray<NetworkConnection> connections;
        public NetworkPipeline pipeline;

        public void Execute(int index) {
            var connection = connections[index];
            NetworkEvent.Type cmd;
            while (NetworkEvent.Type.Empty != (cmd = driver.PopEventForConnection(connection, out var stream))) {
                switch (cmd) {
                    case NetworkEvent.Type.Data: {
                        var number = stream.ReadUInt();
                        Debug.Log($"Got {number} from the Client adding + 2 to it.");
                        number += 2;

                        var writer = driver.BeginSend(pipeline, connection);
                        writer.WriteUInt(number);
                        driver.EndSend(writer);
                        break;
                    }
                    case NetworkEvent.Type.Disconnect:
                        Debug.Log("Client disconnected from server");
                        connections[index] = default(NetworkConnection);
                        break;
                    case NetworkEvent.Type.Empty:
                        break;
                    case NetworkEvent.Type.Connect:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        _jobHandle.Complete();

        var connectJob = new ServerUpdateConnectionsJob {
            driver = _driver,
            connections = _connections
        };

        var updateJob = new ServerUpdateJob {
            driver = _driver.ToConcurrent(),
            connections = _connections.AsDeferredJobArray(),
            pipeline = _pipeline
        };

        _jobHandle = _driver.ScheduleUpdate(inputDeps);
        _jobHandle = connectJob.Schedule(_jobHandle);
        _jobHandle = updateJob.Schedule(_connections, 1, _jobHandle);

        return _jobHandle;
    }
}
