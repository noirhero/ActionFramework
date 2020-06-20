// Copyright 2018-2020 TAP, Inc. All Rights Reserved.

using UnityEngine;
using Unity.Networking.Transport;

public class NetTester : MonoBehaviour {
    private NetworkDriver _driver;
    private NetworkPipeline _pipeline;
    private NetworkConnection _connection;
    private bool done;

    void Start() {
        _driver = NetworkDriver.Create();
        _pipeline = _driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        _connection = _driver.Connect(NetworkEndPoint.Parse("127.0.0.1", 8989));
    }

    public void OnDestroy() {
        _driver.Dispose();
    }

    void Update() {
        _driver.ScheduleUpdate().Complete();

        if (!_connection.IsCreated) {
            if (!done)
                Debug.Log("Something went wrong during connect");
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = _connection.PopEvent(_driver, out stream)) != NetworkEvent.Type.Empty) {
            if (cmd == NetworkEvent.Type.Connect) {
                Debug.Log("We are now connected to the server");

                uint value = 10;
                var writer = _driver.BeginSend(_connection);
                //var writer = _driver.BeginSend(_pipeline, _connection);
                writer.WriteUInt(value);
                _driver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data) {
                Debug.Log($"Got the value = {stream.ReadUInt()} back from the server");
                done = true;
                _connection.Disconnect(_driver);
            }
            else if (cmd == NetworkEvent.Type.Disconnect) {
                Debug.Log("Client got disconnected from server");
                _connection = default;
            }
        }
    }
}
