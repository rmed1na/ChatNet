"use strict";

let started = false;
let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
disableSend();

connection.on("NewMessageReceived", function (user, timestamp, message) {
    let parent = document.getElementById('messagesContainer');
    let chatItem = document.createElement('div');

    chatItem.textContent = `[${timestamp}] ${user}: ${message}`;
    chatItem.classList.add('list__item');

    parent.appendChild(chatItem);
    parent.scrollTo(0, parent.scrollHeight);
});

connection.start().then(function () {
    started = true;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("btnSendMessage").addEventListener("click", function (event) {
    let input = document.getElementById('messageText');
    let message = input.value;
    connection.invoke("SendMessage", message).catch(function (err) {
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

function disableSend() {
    document.getElementById("btnSendMessage").disabled = true;
}
function enableSend() {
    document.getElementById("btnSendMessage").disabled = false;
}