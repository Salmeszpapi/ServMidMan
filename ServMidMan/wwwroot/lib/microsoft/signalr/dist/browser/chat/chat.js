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
    setTimeout(function () {
        popupMessage.style.backgroundColor = "white"; // Reset color after 5 seconds
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
    var nameInput1 = document.getElementById("ProductUploadedButtonForm1");
    var nameInput2 = document.getElementById("ProductUploadedButtonForm2");
    var nameInput3 = document.getElementById("ProductUploadedButtonForm3");
    var nameInput4 = document.getElementById("ProductUploadedButtonForm4");
    var nameInput5 = document.getElementById("ProductUploadedButtonForm5");
    var nameInput6 = document.getElementById("ProductUploadedButtonForm6");

    if (nameInput1.value.trim() === "" ||
        nameInput2.value.trim() === "" ||
        nameInput3.value.trim() === "" ||
        nameInput6.value.trim() === "") {
        alert("Please fill in all fields before submitting the form.");
        return; // Exit the function early if any field is empty
    } else {
        form.submit();
    }
    var user = "12";
    var message = "1";

    var myPopupContainer = document.getElementById("popup-container");
    myPopupContainer.style.display = "Block";
    var myPopup = document.getElementById("popup-message");
    myPopup.style.background = "red";
    connection.invoke("NewProductUpdated", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

