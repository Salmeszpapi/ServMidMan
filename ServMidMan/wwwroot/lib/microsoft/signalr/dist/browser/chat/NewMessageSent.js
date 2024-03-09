"use strict";

document.addEventListener("DOMContentLoaded", function () {
    connection.start().then(function () {
    }).catch(function (err) {
        return console.error(err.toString());
    });

    document.getElementById("sendMessageBtn").addEventListener("click", function (event) {
        var message = "Hello, World!"; // Example message
        var recipientId = "2"; // Example recipient ID

        connection.invoke("SendMessage", message, recipientId).catch(function (err) {
            return console.error(err.toString());
        });
    });
});


