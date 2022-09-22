"use strict";


//Declare Chat area
const m = $('#chat-area')[0];
let bottom = true;

//Connect
const chatId = window.location.pathname.split("/").pop();
const param = $.param({ page: 'chat', chatId });

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub?" + param)
    .build();

connection.onclose(err => {
    alert('Disconnected');
    location = '/';
});


//Detect message at bottom or not
function isBottom() {
    bottom = m.scrollTop + m.clientHeight + 70 >= m.scrollHeight;
}

//Auto scroll down if message at the bottom
function scrollToBottom() {
    if (bottom) {
        m.scrollTop = m.scrollHeight;
    }
}

//Scroll the latest message to bottom when loaded
function directToBottom() {
    $('#chat-area').scrollTop(m.scrollHeight);
}

//GetImageURL(message) --> url

function getImageURL(message) {
    let re = /.(jpg|jpeg|png|webp|gif|bmp)$/i;
    try {
        let url = new URL(message);
        if (re.test(url.pathname)) {
            return url.href;
        }
    }
    catch {
        //Do nothing
    }
    return null;
}

// Convert document to base64
function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });
}

//GetYouTubeId(message) --> id
function getYouTubeId(message) {
    try {
        let url = new URL(message);
        if (url.hostname == 'www.youtube.com' && url.pathname == '/watch') {
            return url.searchParams.get('v');
        }
    }
    catch {
        //Do nothing
    }
    return null;
}

//Screenshot base on Window tab
function screenshot() {
    let canvas = document.createElement('canvas');

    // Request media
    navigator.mediaDevices.getDisplayMedia().then(stream => {
        // Grab frame from stream
        let track = stream.getVideoTracks()[0];
        let capture = new ImageCapture(track);

        let user = $('.your-name').text().trim();
        let profile = $('#profile').attr('src');
        let sentDate = moment().format('DD/MM/YYYY h:mm:ss A');

        capture.grabFrame().then(bitmap => {
            // Stop sharing
            track.stop();

            // Draw the bitmap to canvas
            canvas.width = bitmap.width;
            canvas.height = bitmap.height;
            canvas.getContext('2d').drawImage(bitmap, 0, 0);
            let img = canvas.toDataURL('image/png');

            let data = {
                Word: img,
                ChatID: $('#chat-id').val()

            };

            $.ajax({
                type: "POST",
                url: "/Messages/SendImage",
                data: data,
                success: function (res) {
                    connection.invoke('SendImage', user, img, profile, sentDate);
                }
            });

        });
    })
        .catch(e => console.log(e));
}

//Open camera on modal
function openCamera() {
    Webcam.set({
        width: 320,
        height: 240,
        flip_horiz: true,
        image_format: 'jpeg',
        jpeg_quality: 90
    });
    Webcam.attach('#my_camera');
}

//Capture image from camera
function take_snapshot() {

    let user = $('.your-name').text().trim();
    let profile = $('#profile').attr('src');
    let sentDate = moment().format('DD/MM/YYYY h:mm:ss A');

    // take snapshot and get image data
    Webcam.snap(function (data_uri) {
        let data = {
            Word: data_uri,
            ChatID: $('#chat-id').val()

        };

        $.ajax({
            type: "POST",
            url: "/Messages/SendImage",
            data: data,
            success: function (res) {
                connection.invoke('SendImage', user, data_uri, profile, sentDate);
            }
        });
    });

    Webcam.reset();
}

//Close Camera
function closeCamera() {
    Webcam.reset();
}

//Text-to-hyperlink transform function
function transformHyperlink(message) {
    let re = /(?<=^|\s)(https?:\/\/\S+)(?=$|\s)/gi;
    try {
        if (re.test(message)) {
            return message;
        }
    }
    catch {
        //Do nothing
    }
    return null;


}


//SignalR Returned Message
connection.on("ReceiveMessage", function (user, message, sentDate, who, profile) {

    //Encode HTML Code
    message = $('<div>').text(message).html();

    //Text-to-hyperlink transform
    message = message.replace(
        /(?<=^|\s)(https?:\/\/\S+)(?=$|\s)/gi,
        '<a style="color:mediumseagreen" target="_blank" href="$&">$&</a>'
    );

    //Append SignalR Chat
    if (who == "others") {
        isBottom();
        $('#chat-area').append(` <div id="friends-chat" class="friends-chat">
                                    <div class="profile friends-chat-photo">
                                        <img src="${profile}" alt="">
                                    </div>
                                    <div class="friends-chat-content">
                                        <p class="friends-chat-name">${user}</p>
                                        <p class="friends-chat-balloon">${message}</p>
                                        <h5 class="chat-datetime">${sentDate}</h5>
                                    </div>
                                </div>`);
        scrollToBottom();
    }

    else if (who == "caller") {
        isBottom();
        $('#chat-area').append(`<div id="your-chat" class="your-chat">
                            <p class="your-chat-balloon">${message}</p>
                            <p class="chat-datetime"><span class="glyphicon glyphicon-ok"></span>${sentDate}</p>
                        </div>`);
        scrollToBottom();
    }
});


//SignalR Returned Image
connection.on("ReceiveImage", function (user, url, sentDate, who, profile) {
    if (who == "others") {
        isBottom();
        $('#chat-area').append(` <div id="friends-chat" class="friends-chat">
                                    <div class="profile friends-chat-photo">
                                        <img src="${profile}" alt="">
                                    </div>
                                    <div class="friends-chat-content">
                                        <p class="friends-chat-name">${user}</p>
                                        <img src="${url}" class="friends-chat-image" onload="scrollToBottom()">
                                        <h5 class="chat-datetime">${sentDate}</h5>
                                    </div>
                                </div>`);

    }

    else if (who == "caller") {
        isBottom();
        $('#chat-area').append(`<div id="your-chat" class="your-chat">
                               <img src="${url}" class="your-chat-image" onload="scrollToBottom()">
                            <p class="chat-datetime"><span class="glyphicon glyphicon-ok"></span>${sentDate}</p>
                        </div>`);
    }

});

//SignalR Returned Document
connection.on("ReceiveDocument", function (user, document, sentDate, who, profile) {
    if (who == "others") {
        isBottom();
        $('#chat-area').append(` <div id="friends-chat" class="friends-chat">
                                    <div class="profile friends-chat-photo">
                                        <img src="${profile}" alt="">
                                    </div>
                                    <div class="friends-chat-content">
                                        <p class="friends-chat-name">${user}</p>
                                     <p class="friends-chat-balloon"><a href="${document}" style="color:plum;" target="_blank" download="Document">Download the document</a></p>
                                        <h5 class="chat-datetime">${sentDate}</h5>
                                    </div>
                                </div>`);
        scrollToBottom();

    }

    else if (who == "caller") {
        isBottom();
        $('#chat-area').append(`<div id="your-chat" class="your-chat">
                                   <p class="your-chat-balloon"><a href="${document}" style="color:plum;" target="_blank" download="Document">Download the documents</a></p>
                            <p class="chat-datetime"><span class="glyphicon glyphicon-ok"></span>${sentDate}</p>
                        </div>`);
        scrollToBottom();
    }

});

//SignalR Returned Youtube
connection.on("ReceiveVideo", function (user, id, sentDate, who, profile) {
    if (who == "others") {
        isBottom();
        $('#chat-area').append(` <div id="friends-chat" class="friends-chat">
                                    <div class="profile friends-chat-photo">
                                        <img src="${profile}" alt="">
                                    </div>
                                    <div class="friends-chat-content">
                                        <p class="friends-chat-name">${user}</p>
                                     <iframe class="friends-chat-video" width="400" height="300"
                                        src="https://www.youtube.com/embed/${id}"
                                        frameborder="0"
                                        allow="accelerometer; autoplay; clipboard-write; encrypted-media;
                                        gyroscope; picture-in-picture"
                                        allowfullscreen></iframe>
                                        <h5 class="chat-datetime">${sentDate}</h5>
                                    </div>
                                </div>`);
        scrollToBottom();

    }

    else if (who == "caller") {
        isBottom();
        $('#chat-area').append(`<div id="your-chat" class="your-chat">
                                   <iframe class="your-chat-video" width="400" height="300"
                                    src="https://www.youtube.com/embed/${id}"
                                    frameborder="0"
                                    allow="accelerometer; autoplay; clipboard-write; encrypted-media;
                                    gyroscope; picture-in-picture"
                                    allowfullscreen></iframe>
                            <p class="chat-datetime"><span class="glyphicon glyphicon-ok"></span>${sentDate}</p>
                        </div>`);
        scrollToBottom();
    }

});

//SignalR Returned Audio
connection.on("ReceiveAudio", function (user, audio, sentDate, who, profile) {
    if (who == "others") {
        isBottom();
        $('#chat-area').append(` <div id="friends-chat" class="friends-chat">
                                    <div class="profile friends-chat-photo">
                                        <img src="${profile}" alt="">
                                    </div>
                                    <div class="friends-chat-content">
                                        <p class="friends-chat-name">${user}</p>
                                        <p class="friends-chat-balloon"><audio controls src="${audio}"></audio></p>
                                        <h5 class="chat-datetime">${sentDate}</h5>
                                    </div>
                                </div>`);
        scrollToBottom();

    }

    else if (who == "caller") {
        isBottom();
        $('#chat-area').append(`<div id="your-chat" class="your-chat">
                                   <p class="your-chat-balloon"><audio controls src="${audio}"></audio></p>
                            <p class="chat-datetime"><span class="glyphicon glyphicon-ok"></span>${sentDate}</p>
                        </div>`);
        scrollToBottom();
    }

});

//SignalR Returned Cam Video
connection.on("ReceiveCamVideo", function (user, video, sentDate, who, profile) {
    if (who == "others") {
        isBottom();
        $('#chat-area').append(` <div id="friends-chat" class="friends-chat">
                                    <div class="profile friends-chat-photo">
                                        <img src="${profile}" alt="">
                                    </div>
                                    <div class="friends-chat-content">
                                        <p class="friends-chat-name">${user}</p>
                                        <p class="friends-chat-balloon"><video class="friends-chat-image" src="${video}" controls></video></p>
                                        <h5 class="chat-datetime">${sentDate}</h5>
                                    </div>
                                </div>`);
        scrollToBottom();

    }

    else if (who == "caller") {
        isBottom();
        $('#chat-area').append(`<div id="your-chat" class="your-chat">
                                   <p class="your-chat-balloon"><video class="your-chat-image" src="${video}" controls></video></audio></p>
                            <p class="chat-datetime"><span class="glyphicon glyphicon-ok"></span>${sentDate}</p>
                        </div>`);
        scrollToBottom();
    }

});

//Start Connection of SignalR
connection.start().then(main);


//===========================================
//Trigger main function after connection start
//============================================
function main() {

    //Scroll chat to bottom while drop file
    directToBottom();

    //Submit Message from form
    $("#messageForm").submit(function (event) {
        event.preventDefault();

        let user = $('.your-name').text().trim();
        let message = $('#type-area').val().trim();
        let profile = $('#profile').attr('src');
        let sentDate = moment().format('DD/MM/YYYY h:mm:ss A');

        //Empty Message Return
        if (!message) {
            return false;
        }

        //Check message contains Image URL
        let url = getImageURL(message);


        //Check message contains Youtube ID
        let id = getYouTubeId(message);

        //Check message contains Hyperlink
        let hyperlink = transformHyperlink(message);

        //Checking Message Type
        if (url) {
            $('#message-type').val("Image");
        }
        else if (id) {
            $('#message-type').val("Video");
            $('#video-url').val(id);
        }
        else if (hyperlink) {
            $('#message-type').val("Hyperlink");
        }
        else {
            $('#message-type').val("Text");
        }

        //Ajax Form Post
        let form = $('#messageForm');
        let formUrl = form.attr("action");
        let formData = form.serialize();
        $.post(formUrl, formData, function (data) {

            //Clear Message and Focus
            $('#type-area').val('').focus();
            $('#message-type').val('');
            $('#video-url').val('');

            if (url) {
                //Send Image to Hub
                connection.invoke('SendImage', user, url, profile, sentDate).catch(function (err) {
                    return console.error(err.toString());
                });
            }

            else if (id) {
                //Send Youtube to Hub
                connection.invoke('SendVideo', user, id, profile, sentDate).catch(function (err) {
                    return console.error(err.toString());
                });
            }

            else {
                //Send Message to Hub
                connection.invoke("SendMessage", user, message, profile, sentDate).catch(function (err) {
                    return console.error(err.toString());
                });
            }

        });



    });

}

//Select image from file
$('#image').click(e => $('#file').click());

//Drag-and-drop file select
$('#chat-area').on('dragenter dragover', e => {
    e.preventDefault();
    $('#chat-area').addClass('active');

});

$('#chat-area').on('drageleave drop', e => {
    e.preventDefault();
    $('#chat-area').removeClass('active');
});

$('#chat-area').on('drop', function (e) {
    e.preventDefault();
    var reader = new FileReader();
    reader.onload = function (event) {
        var img = new Image();
        img.onload = function () {
            var max_size = 544;
            var width = img.width;
            var height = img.height;

            if (width > height) {
                if (width > max_size) {
                    height *= max_size / width;
                    width = max_size;
                }
            } else {
                if (height > max_size) {
                    width *= max_size / height;
                    height = max_size;
                }
            }
            can.width = width;
            can.height = height;
            ctx.drawImage(img, 0, 0, width, height);
            can.toDataURL
        }
        img.src = event.target.result;
    }
    reader.readAsDataURL(e.originalEvent.dataTransfer.files[0]);
    $('#canvasModal').modal('show');
    e.target.value = null;
});


//Request image fullscreen
$('#chat-area').on('click', '.your-chat-image', e => e.target.requestFullscreen());
$('#chat-area').on('click', '.friends-chat-image', e => e.target.requestFullscreen());

connection.on('UpdateStatus', (chatId, user, message) => {
    $(`#${chatId}`).text(`${user} : ${message}`);
});

connection.on('UpdateNotify', (chatId) => {

    if ($(`#${chatId}-notify`).text() == "") {
        $(`#${chatId}-notify`).text("1");
    }
    else {
        let num = 1 + parseInt($(`#${chatId}-notify`).text())
        $(`#${chatId}-notify`).text(num.toString());
    }
    var myAudio = new Audio("../../audio/notification.mp3");
    myAudio.play();
});

$('#main-right').click(e => {
    $(`#${chatId}-notify`).text("");
});



//Audio to Text
let txtMessage = document.querySelector('#type-area');
var SpeechRecognition = SpeechRecognition || webkitSpeechRecognition;
var SpeechGrammarList = SpeechGrammarList || webkitSpeechGrammarList;
var grammar = '#JSGF V1.0;'
var recognition = new SpeechRecognition();
var speechRecognitionList = new SpeechGrammarList();
speechRecognitionList.addFromString(grammar, 1);
recognition.grammars = speechRecognitionList;
recognition.lang = 'en-US';
recognition.interimResults = true;
recognition.onresult = function (event) {
    let command = event.results[0][0].transcript;
    let isFinal = event.results[0].isFinal;
    $('#audioText').text(command);
    if (isFinal) {
        //$('#audioText').text(command);
        //txtMessage.value += command;
    }
}

document.querySelector('#audioToTextAgain').onclick = function () {
    $('#audioText').text("...");
    recognition.start();
};

document.querySelector('#start-recording').onclick = function () {
    $('#audioText').text("...");
    recognition.start();
};


$('#stop-recording').click(e => {
    recognition.stop();
});

//     recognition.onspeechend = function () {
//         recognition.stop();
//     };


recognition.onerror = function (event) {
    $('#audioText').text("Error occurred in recognition: " + event.error);
}


$('#confirmText').click(e => {
    txtMessage.value = $('#audioText').text();
    txtMessage.focus();
});




// Canvas Input ==============================================================
$('#size').on('input', e => {
    let size = $('#size').val();
    $('#pen').width(size).height(size);
});

$('#color').on('input', e => {
    let color = $('#color').val();
    $('#pen').css('background', color);
});

$('#pens').on('input', e => {
    let pens = $('#pens').val();
});

$('#dashed').on('input', e => {
    let dashed = $('#dashed').val();
});

$('#size, #color, #pens, #dashed').trigger('input');

$('#canvas').on('wheel', e => {
    e.preventDefault();
    let dy = e.originalEvent.deltaY;
    if (dy < 0) {
        $('#size')[0].stepUp();
    }
    else {
        $('#size')[0].stepDown();
    }
    $('#size').trigger('input');
});

//Right Click
$('#canvas').on('contextmenu', e => {
    e.preventDefault();
});

$(document).keydown(e => {
    if (e.originalEvent.repeat) return;

    switch (e.key) {
        case '1': $('#color').val('#ff0000'); break;
        case '2': $('#color').val('#ffa500'); break;
        case '3': $('#color').val('#ffff00'); break;
        case '4': $('#color').val('#008000'); break;
        case '5': $('#color').val('#0000ff'); break;
        case '6': $('#color').val('#4b0082'); break;
        case '7': $('#color').val('#ee82ee'); break;
        case '8': $('#color').val('#000000'); break;
        case '9': $('#color').val('#ffffff'); break;
    }
    $('#color').trigger('input');
});




//Canvas Draw Functions ==================================================================
const can = $('#canvas')[0];
const ctx = can.getContext('2d');

function drawLine(a, b, style) {
    ctx.beginPath();
    ctx.moveTo(a.x, a.y);
    ctx.lineTo(b.x, b.y);
    ctx.lineWidth = style.size;
    ctx.strokeStyle = style.color;
    ctx.shadowColor = style.color;
    ctx.lineCap = style.lineCap;
    ctx.lineJoin = style.lineJoin;
    ctx.shadowBlur = style.shadowBlur;
    ctx.setLineDash([style.lineDash1, style.lineDash2]);
    ctx.globalAlpha = style.globalAlpha;
    ctx.stroke();
}

function drawCurve(a, b, c, style) {
    ctx.beginPath();
    ctx.moveTo(a.x, a.y);
    ctx.quadraticCurveTo(b.x, b.y, c.x, c.y);
    ctx.lineWidth = style.size;
    ctx.strokeStyle = style.color;
    ctx.shadowColor = style.color;
    ctx.lineCap = style.lineCap;
    ctx.lineJoin = style.lineJoin;
    ctx.shadowBlur = style.shadowBlur;
    ctx.setLineDash([style.lineDash1, style.lineDash2]);
    ctx.globalAlpha = style.globalAlpha;
    ctx.stroke();
}


function mid(a, b) {
    return { x: (a.x + b.x) / 2, y: (a.y + b.y) / 2 };
}

function cursor() {
    if ($('#pens').val() * 1 == 1) {
        $('#canvas').css('cursor', 'url("../../images/pencil-down.png") 0 32, pointer');
    }
    else if ($('#pens').val() * 1 == 2) {
        $('#canvas').css('cursor', 'url("../../images/pen.png") 0 32, pointer');
    }
    else if ($('#pens').val() * 1 == 3) {
        $('#canvas').css('cursor', 'url("../../images/highlight.png") 0 32, pointer');
    }

}

// Canvas Draw Events ========================================================================
let arr = [];

//Pointer Move
$('#canvas').on('pointermove pointerout', e => {
    cursor();
    if (e.buttons == 1 && e.originalEvent.isPrimary) {

        // let style = {size,color,lineCap,lineJoin,shadowBlur,LineDash1,LineDash2,GlobalAlpha};
        let style = {
            size: 5,
            color: '#FFFFFF',
            lineCap: 'round',
            lineJoin: 'round',
            shadowBlur: 0,
            lineDash1: 0,
            lineDash2: 0,
            globalAlpha: 1
        };

        style.size = $('#size').val() * 1;
        style.color = $('#color').val();

        let pens = $('#pens').val() * 1;
        let dashed = $('#dashed').val() * 1;


        style.lineCap = "round";
        style.lineJoin = "round";
        style.shadowBlur = 0;
        style.shadowBlur = 0;

        if (pens == 1) {
            style.lineCap = "round";
            style.lineJoin = "round";
            style.shadowBlur = 0;
            style.globalAlpha = 1;
        }
        else if (pens == 2) {
            style.lineCap = "round";
            style.lineJoin = "round";
            style.shadowBlur = 5;
            style.globalAlpha = 0.8;
        }
        else if (pens == 3) {
            style.lineCap = "butt";
            style.lineJoin = "butt";
            style.shadowBlur = 0;
            style.globalAlpha = 0.4;
        }

        if (dashed == 1) {
            style.lineDash1 = 0;
            style.lineDash2 = 0;
        }
        else if (dashed == 2) {
            style.lineDash1 = 10;
            style.lineDash2 = 10;
        }
        else if (dashed == 3) {
            style.lineDash1 = 20;
            style.lineDash2 = 5;
        }

        // 1. Add start point (if needed)
        if (arr.length == 0) {
            arr.push({ x: e.offsetX - e.originalEvent.movementX, y: e.offsetY - e.originalEvent.movementY });
        }

        // 2. Add new point
        arr.push({ x: e.offsetX, y: e.offsetY });
        arr = arr.slice(-3); // Keep last 3 points only

        if (arr.length >= 3) {
            // arr = [0, 1, 2]
            let a = mid(arr[0], arr[1]);
            let b = arr[1];
            let c = mid(arr[1], arr[2]);
            drawCurve(a, b, c, style);
        }
        else {
            // arr = [0, 1]
            let a = arr[0];
            let b = mid(arr[0], arr[1]);
            drawLine(a, b, style);
        }
    }

    else if (e.buttons == 2 && e.originalEvent.isPrimary) {
        $('#canvas').css('cursor', 'url("../../images/eraser.png") 0 22, pointer');

        let style = {
            size: $('#size').val() * 1,
            color: '#FFFFFF',
            lineCap: 'round',
            lineJoin: 'round',
            shadowBlur: 0,
            lineDash1: 0,
            lineDash2: 0,
            globalAlpha: 1
        };

        // 1. Add start point (if needed)
        if (arr.length == 0) {
            arr.push({ x: e.offsetX - e.originalEvent.movementX, y: e.offsetY - e.originalEvent.movementY });
        }

        // 2. Add new point
        arr.push({ x: e.offsetX, y: e.offsetY });
        arr = arr.slice(-3); // Keep last 3 points only

        if (arr.length >= 3) {
            // arr = [0, 1, 2]
            let a = mid(arr[0], arr[1]);
            let b = arr[1];
            let c = mid(arr[1], arr[2]);
            drawCurve(a, b, c, style);
        }
        else {
            // arr = [0, 1]
            let a = arr[0];
            let b = mid(arr[0], arr[1]);
            drawLine(a, b, style);
        }
    }
    //Coord
    var message = 'X:' + Math.round(e.offsetX) + ' Y:' + Math.round(e.offsetY);
    coord.innerHTML = message;
});


//Pointer Out
$('#canvas').on('pointerup pointerout', e => {
    cursor();

    let style = {
        size: $('#size').val() * 1,
        color: '#FFFFFF',
        lineCap: 'round',
        lineJoin: 'round',
        shadowBlur: 0,
        lineDash1: 0,
        lineDash2: 0,
        globalAlpha: 1
    };


    if (e.buttons == 1 && e.originalEvent.isPrimary) {
        style.size = $('#size').val() * 1;
        style.color = $('#color').val();
        let pens = $('#pens').val() * 1;
        let dashed = $('#dashed').val() * 1;

        style.lineCap = "round";
        style.lineJoin = "round";
        style.shadowBlur = 0;
        style.shadowBlur = 0;


        if (pens == 1) {
            style.lineCap = "round";
            style.lineJoin = "round";
            style.shadowBlur = 0;
            style.globalAlpha = 1;
        }
        else if (pens == 2) {
            style.lineCap = "round";
            style.lineJoin = "round";
            style.shadowBlur = 5;
            style.globalAlpha = 0.8;
        }
        else if (pens == 3) {
            style.lineCap = "butt";
            style.lineJoin = "butt";
            style.shadowBlur = 0;
            style.globalAlpha = 0.4;
        }

        if (dashed == 1) {
            style.lineDash1 = 0;
            style.lineDash2 = 0;
        }
        else if (dashed == 2) {
            style.lineDash1 = 10;
            style.lineDash2 = 10;
        }
        else if (dashed == 3) {
            style.lineDash1 = 20;
            style.lineDash2 = 5;
        }

        if (arr.length >= 2) {
            arr = arr.slice(-2); // Keep last 2 points only

            // arr = [0, 1]
            let a = mid(arr[0], arr[1]);
            let b = arr[1];
            drawLine(a, b, style);
            // TODO(C): Hub method
        }
    }


    arr = [];
    //Coord
    coord.innerHTML = 'X: Y:';

});

$('#download').click(e => {
    let a = $('<a>')[0];
    a.href = can.toDataURL('image/png');
    a.download = Date.now() + '.png';
    a.click();
});

$('#cropCanvas').click(e => {
    let url = can.toDataURL('image/png');

    $image_crop.croppie('bind', {
        url: url
    }).then(function () {
        console.log('jQuery bind complete');
    });

    $('#uploadimageModal').modal('show');

})

$('#sendCanvas').click(async (e) => {

    let user = $('.your-name').text().trim();
    let profile = $('#profile').attr('src');
    let sentDate = moment().format('DD/MM/YYYY h:mm:ss A');
    let url = can.toDataURL('image/png');

    let data = {
        Word: url,
        ChatID: $('#chat-id').val()

    };

    $.ajax({
        type: "POST",
        url: "/Messages/SendImage",
        data: data,
        success: function (res) {
            connection.invoke('SendImage', user, url, profile, sentDate);
        }
    })
    $('#canvasModal').modal('hide');

    //Scroll chat to bottom while drop file
    directToBottom();
});
//Select document from file
$('#document').click(e => $('#attachment').click());

$('#attachment').change(async (e) => {
    let user = $('.your-name').text().trim();
    let profile = $('#profile').attr('src');
    let sentDate = moment().format('DD/MM/YYYY h:mm:ss A');

    let f = e.target.files[0];

    let document = await getBase64(f);

    let data = {
        Word: document,
        ChatID: $('#chat-id').val()

    };

    $.ajax({
        type: "POST",
        url: "/Messages/SendAttachment",
        data: data,
        success: function (res) {
            connection.invoke('SendDocument', user, document, profile, sentDate);
        }
    })

});

$('#confirmVoice').click(e => {
    let user = $('.your-name').text().trim();
    let profile = $('#profile').attr('src');
    let sentDate = moment().format('DD/MM/YYYY h:mm:ss A');
    let audio = $('#confirmVoice').val();


    let data = {
        Word: audio,
        ChatID: $('#chat-id').val()

    };

    $.ajax({
        type: "POST",
        url: "/Messages/SendAudio",
        data: data,
        success: function (res) {
            console.log(res.response);
            connection.invoke('SendAudio', user, audio, profile, sentDate);
        }
    })

});

//Audio Recording
function captureUserMedia(mediaConstraints, successCallback, errorCallback) {
    navigator.mediaDevices.getUserMedia(mediaConstraints).then(successCallback).catch(errorCallback);
}

var mediaConstraints = {
    audio: true
};
document.querySelector('#save-recording').onclick = function () {
    this.disabled = true;
    mediaRecorder.save();
};
function startRecording2() {
    $('#start-recording').disabled = true;
    captureUserMedia(mediaConstraints, onMediaSuccess, onMediaError);
};

function stopRecording2() {
    $('#stop-recording').disabled = true;
    mediaRecorder.stop();
    mediaRecorder.stream.stop();

    $('.start-recording').disabled = false;
};

function saveRecording2() {
    mediaRecorder.save();
};

var mediaRecorder;

async function onMediaSuccess(stream) {
    mediaRecorder = new MediaStreamRecorder(stream);
    mediaRecorder.stream = stream;
    mediaRecorder.mimeType = 'audio/wav';
    mediaRecorder.audioChannels = 1;
    mediaRecorder.ondataavailable = async function (blob) {
        $('#record-audio').html("<audio id='audioRecorded' controls=''><source src=" + URL.createObjectURL(blob) + "></source></audio>");
        let audio = await getBase64(blob);
        $('#confirmVoice').val(audio);
    };

    var timeInterval = 360 * 1000;

    mediaRecorder.start(timeInterval);

    $('#stop-recording').disabled = false;
}

function onMediaError(e) {
    console.error('media error', e);
}

function bytesToSize(bytes) {
    var k = 1000;
    var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    if (bytes === 0) return '0 Bytes';
    var i = parseInt(Math.floor(Math.log(bytes) / Math.log(k)), 10);
    return (bytes / Math.pow(k, i)).toPrecision(3) + ' ' + sizes[i];
}

function getTimeLength(milliseconds) {
    var data = new Date(milliseconds);
    return data.getUTCHours() + " hours, " + data.getUTCMinutes() + " minutes and " + data.getUTCSeconds() + " second(s)";
}

window.onbeforeunload = function () {
    $('#start-recording').disabled = false;
};

$('#micModal').on('hidden.bs.modal', function () {
    $('#confirmVoice').val('');
    $('#audioRecorded').src = null;
});