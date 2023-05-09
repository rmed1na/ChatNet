"use strict";

let started = false;
let roomId = parseInt(document.getElementById('chatContainer').getAttribute('data-roomId'));
let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
disableSend();

// Hub methods
connection.start().then(function () {
    started = true;
    subscribeToChatroom();
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("NewMessageReceived", function (message) {
    let parent = document.getElementById('messagesContainer');
    let chatItem = document.createElement('div');

    chatItem.textContent = message;
    chatItem.classList.add('list__item');
    parent.appendChild(chatItem);
    parent.scrollTo(0, parent.scrollHeight);
});

connection.on("HubException", function (ex) {
    console.error('The chatHub has thrown an exception', ex);
});


// Event listeners
document.getElementById("btnSendMessage").addEventListener("click", function (event) {
    let input = document.getElementById('messageText');
    let message = input.value;
    connection.invoke("SendMessage", message, roomId).catch(function (err) {
        return console.error(err.toString());
    });
    input.value = '';
    disableSend();
    event.preventDefault();
});
document.getElementById('messageText').addEventListener('keyup', function (event) {
    if (started && this.value && this.value.length > 0)
        enableSend();
    else
        disableSend();
});

// Helpers
function disableSend() {
    document.getElementById("btnSendMessage").disabled = true;
}

function enableSend() {
    document.getElementById("btnSendMessage").disabled = false;
}

function subscribeToChatroom() {
    if (isNaN(roomId)) {
        console.warn(`Can't subscribe to chatroom. Integer conversion failed. (${roomId})`);
        return;
    }

    connection.invoke("SubscribeToChatroom", roomId).catch(function (err) {
        return console.error(err.toString());
    });
}