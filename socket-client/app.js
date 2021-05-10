const signalR = require("@microsoft/signalr")

var competitorId = "43345403-4a63-4979-8486-22971614f7b6"
var userId = "53345403-4a63-4979-8486-22971614f7b6"
var bearerToken = "token"

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/testhub", { accessTokenFactory: () => bearerToken })
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("helloClient", (user, message) => {
        console.log(user)
        console.log(message)
    });

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
        connection.invoke("Register", userId, competitorId).catch(function (err) {
            return console.error(err.toString());
        });
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(start);

// Start the connection.
start();
