let recordedBlobs;

const codecPreferences = document.querySelector('#codecPreferences');
const errorMsgElement = document.querySelector('span#errorMsg');
const recordedVideo = document.querySelector('video#recorded');
const recordButton = document.querySelector('button#record');

recordButton.addEventListener('click', () => {
    if (recordButton.textContent === 'Start Recording') {
        startRecording();
    } else {
        stopRecording();
        recordButton.textContent = 'Start Recording';
        playButton.disabled = false;
        codecPreferences.disabled = false;
    }
});

const playButton = document.querySelector('button#play');
playButton.addEventListener('click', () => {
    const mimeType = codecPreferences.options[codecPreferences.selectedIndex].value.split(';', 1)[0];
    const superBuffer = new Blob(recordedBlobs, { type: mimeType });
    recordedVideo.src = null;
    recordedVideo.srcObject = null;
    recordedVideo.src = window.URL.createObjectURL(superBuffer);
    recordedVideo.hidden = false;
    recordedVideo.controls = true;
    recordedVideo.play();
});


async function handleDataAvailable(event) {
    console.log('handleDataAvailable', event);
    if (event.data && event.data.size > 0) {
        recordedBlobs.push(event.data);
        let video = await getBase64(event.data);
        $('#confirmVideo').val(video);
    }
}

function getSupportedMimeTypes() {
    const possibleTypes = [
        'video/webm;codecs=vp8,opus',
        'video/webm;codecs=vp9,opus',
        'video/webm;codecs=h264,opus',
        'video/mp4;codecs=h264,aac',
    ];
    return possibleTypes.filter(mimeType => {
        return MediaRecorder.isTypeSupported(mimeType);
    });
}

function startRecording() {
    recordedBlobs = [];
    const mimeType = codecPreferences.options[codecPreferences.selectedIndex].value;
    const options = { mimeType };

    try {
        mediaRecorder = new MediaRecorder(window.stream, options);
    } catch (e) {
        console.error('Exception while creating MediaRecorder:', e);
        errorMsgElement.innerHTML = `Exception while creating MediaRecorder: ${JSON.stringify(e)}`;
        return;
    }

    console.log('Created MediaRecorder', mediaRecorder, 'with options', options);
    recordButton.textContent = 'Stop Recording';
    playButton.disabled = true;
    codecPreferences.disabled = true;
    mediaRecorder.onstop = (event) => {
        console.log('Recorder stopped: ', event);
        console.log('Recorded Blobs: ', recordedBlobs);
    };
    mediaRecorder.ondataavailable = handleDataAvailable;
    mediaRecorder.start();
    console.log('MediaRecorder started', mediaRecorder);
}

function stopRecording() {
    mediaRecorder.stop();
}

function handleSuccess(stream) {
    recordButton.disabled = false;
    console.log('getUserMedia() got stream:', stream);
    window.stream = stream;

    const gumVideo = document.querySelector('video#gum');
    gumVideo.srcObject = stream;

    getSupportedMimeTypes().forEach(mimeType => {
        const option = document.createElement('option');
        option.value = mimeType;
        option.innerText = option.value;
        codecPreferences.appendChild(option);
    });
    codecPreferences.disabled = false;
}

async function init(constraints) {
    try {
        const stream = await navigator.mediaDevices.getUserMedia(constraints);
        handleSuccess(stream);
    } catch (e) {
        console.error('navigator.getUserMedia error:', e);
        errorMsgElement.innerHTML = `navigator.getUserMedia error:${e.toString()}`;
    }
}

document.querySelector('button#start').addEventListener('click', async () => {
    document.querySelector('button#start').disabled = true;
    const hasEchoCancellation = document.querySelector('#echoCancellation').checked;
    const constraints = {
        audio: {
            echoCancellation: { exact: hasEchoCancellation }
        },
        video: {
            width: 1280, height: 720
        }
    };
    console.log('Using media constraints:', constraints);
    await init(constraints);
});


$('#videoModal').on('hidden.bs.modal', function () {
    mediaRecorder.stream.stop();
    document.querySelector('button#start').disabled = false;
    mediaRecorder.src = null;
    document.querySelector('video#gum').srcObject = null;
    recordedVideo.src = "";
    recordedVideo.srcObject = null;
    document.querySelector('video#recorded').hidden = true;
    document.querySelector('button#play').disabled = true;
    document.querySelector('button#record').disabled = true;
    $('#confirmVideo').val("");
});


$('#confirmVideo').click(e => {
    let user = $('.your-name').text().trim();
    let profile = $('#profile').attr('src');
    let sentDate = moment().format('DD/MM/YYYY h:mm:ss A');
    let video = $('#confirmVideo').val().replace('codecs=vp8,opus;', '')


    let data = {
        Word: video,
        ChatID: $('#chat-id').val()

    };

    $.ajax({
        type: "POST",
        url: "/Messages/SendCamVideo",
        data: data,
        success: function (res) {
            console.log(res.response);
            connection.invoke('SendCamVideo', user, video, profile, sentDate);
        }
    })

});

// Convert document to base64
function getBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });
}