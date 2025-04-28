mergeInto(LibraryManager.library, {
    InitializeVideoRecorder: function() {
        try {
            console.log("Initializing video recorder");
            // Store reference to this context for later use
            window.unityRecorder = {
                mediaRecorder: null,
                recordedChunks: []
            };
            return true;
        } catch (e) {
            console.error("Error initializing recorder:", e);
            return false;
        }
    },

    StartVideoRecording: function() {
        try {
            var canvas = document.getElementById("unity-canvas");
            if (!canvas) {
                // Try alternate canvas IDs that Unity might use
                canvas = document.querySelector("canvas");
                if (!canvas) {
                    console.error("Cannot find Unity canvas");
                    return false;
                }
            }
            
            console.log("Found canvas:", canvas);
            
            // Create a stream from the canvas
            var stream = canvas.captureStream(30); // 30 FPS
            
            // Create MediaRecorder instance
            var recorder;
            if (MediaRecorder.isTypeSupported("video/webm;codecs=vp9")) {
                recorder = new MediaRecorder(stream, {mimeType: "video/webm;codecs=vp9"});
            } else if (MediaRecorder.isTypeSupported("video/webm;codecs=vp8")) {
                recorder = new MediaRecorder(stream, {mimeType: "video/webm;codecs=vp8"});
            } else {
                recorder = new MediaRecorder(stream);
            }
            
            // Set up event handlers
            window.unityRecorder.recordedChunks = [];
            
            recorder.ondataavailable = function(e) {
                if (e.data.size > 0) {
                    window.unityRecorder.recordedChunks.push(e.data);
                }
            };
            
            recorder.onstop = function() {
                console.log("Recording stopped, chunks:", window.unityRecorder.recordedChunks.length);
            };
            
            // Start recording
            recorder.start(100); // collect 100ms chunks
            window.unityRecorder.mediaRecorder = recorder;
            console.log("Recording started");
            return true;
        } catch (e) {
            console.error("Error starting recording:", e);
            return false;
        }
    },
        StopVideoRecording: function() {
        if (!window.unityRecorder || !window.unityRecorder.mediaRecorder || 
            window.unityRecorder.mediaRecorder.state !== "recording") {
            console.error("Not recording");
            return false;
        }
        
        try {
            window.unityRecorder.mediaRecorder.stop();
            console.log("Recording stopped");
            return true;
        } catch (e) {
            console.error("Error stopping recording:", e);
            return false;
        }
    },
    
    // Save the recorded video
    SaveVideoRecording: function() {
        if (!window.unityRecorder || !window.unityRecorder.recordedChunks || 
            window.unityRecorder.recordedChunks.length === 0) {
            console.error("No recording available");
            return false;
        }
        
        try {
            var blob = new Blob(window.unityRecorder.recordedChunks, {type: "video/webm"});
            var url = URL.createObjectURL(blob);
            
            var a = document.createElement("a");
            document.body.appendChild(a);
            a.style = "display: none";
            a.href = url;
            a.download = "unity_recording_" + new Date().toISOString().replace(/[:.]/g, '-') + ".webm";
            a.click();
            
            // Clean up
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
            
            console.log("Recording saved");
            return true;
        } catch (e) {
            console.error("Error saving recording:", e);
            return false;
        }
    }

});
