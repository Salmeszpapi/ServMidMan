"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


document.addEventListener("DOMContentLoaded", function () {
    connection.start().then(function () {
    }).catch(function (err) {
        return console.error(err.toString());
    });

    document.getElementById("ProductUploadedButton").addEventListener("click", function (event) {
        event.preventDefault();

        // Trigger form submission
        var form = document.getElementById("ProductUploadedButtonForm");

        form.submit();

        var myPopupContainer = document.getElementById("popup-container");
        myPopupContainer.style.display = "Block";
        var myPopup = document.getElementById("popup-message");
        connection.invoke("NewProductUpdated", "1", "2").catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
});


