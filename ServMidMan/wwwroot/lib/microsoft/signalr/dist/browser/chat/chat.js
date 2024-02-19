"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;
document.getElementById("ProductUploadedButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});
connection.on("ChangeColor", function (color) {
    var popupMessage = document.getElementById("popup-message");
    popupMessage.style.backgroundColor = "green";
    var myPopupContainer = document.getElementById("popup-container");
    myPopupContainer.style.display = "Block";
    myPopupContainer.innerText = "New product has been added";
    setTimeout(function () {
        popupMessage.style.backgroundColor = "white"; // Reset color after 5 seconds
        myPopupContainer.style.display = "None";
    }, 5000);
});

connection.on("ReceivedMessage", function() {
    var popupMessage = document.getElementById("popup-message");
    popupMessage.style.backgroundColor = "green";
    var myPopupContainer = document.getElementById("popup-container");
    myPopupContainer.style.display = "Block";
    myPopupContainer.innerText = "New message has been added";
    setTimeout(function () {
        popupMessage.style.backgroundColor = "white"; // Reset color after 5 seconds
        myPopupContainer.style.display = "None";
    }, 5000);
});
connection.on("NewProductUpdatedReceived", function (user,fero) {

});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    document.getElementById("ProductUploadedButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("ProductUploadedButton").addEventListener("click", function (event) {
    event.preventDefault();

    // Trigger form submission
    var form = document.getElementById("ProductUploadedButtonForm");

    form.submit();

    var myPopupContainer = document.getElementById("popup-container");
    myPopupContainer.style.display = "Block";
    var myPopup = document.getElementById("popup-message");
    connection.invoke("NewProductUpdated", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("sendMessageBtn").addEventListener("click", function (event) {
    var user = document.getElementById("ChatWithCurrentUser").innerText;
    var myPopupContainer = document.getElementById("popup-container");
    myPopupContainer.style.display = "Block";
    var myPopup = document.getElementById("popup-message");
    connection.invoke("NewChatIncomed", user).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

