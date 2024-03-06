"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});


connection.on("ChangeColor", function (color) {

    var myPopupContainer = document.getElementById("popup-container");
    myPopupContainer.style.display = "Block";
    var popupMessage = document.getElementById("popup-message");
    popupMessage.innerText = "New product has been added";
    if (popupMessage) {
        // The element exists, so it's safe to set its properties
        popupMessage.style.backgroundColor = "green";
    } else {
        // The element doesn't exist or couldn't be found
        console.error("Element with ID 'popup-message' not found.");
    }
    setTimeout(function () {
        popupMessage.style.backgroundColor = "white"; // Reset color after 5 seconds
        myPopupContainer.style.display = "none";
    }, 5000);
});