"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
connection.start().then(function () {
    connection.invoke("GetClientSignalRId").catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});


connection.on("ChangeColor", function (newProductID) {

    var myPopupContainer = document.getElementById("popup-container");
    var popupMessage = document.getElementById("popup-message");
    popupMessage.innerHTML = "New product has been added: <a href='https://localhost:7131/Home/Product?id=" + newProductID + "' style='color: white;'>View Product</a>";
    myPopupContainer.style.display = "Block";
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

connection.on("ReceiveMessage", function (SenderId) {

    var myPopupContainer = document.getElementById("popup-container");
    var popupMessage = document.getElementById("popup-message");
    popupMessage.innerHTML = "You got new message : <a href='https://localhost:7131/Chat/Index' style='color: white;'>View Message</a>";
    myPopupContainer.style.display = "Block";
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