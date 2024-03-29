﻿//https://kongaraju.blogspot.com/2012/07/large-file-upload-more-than-1gb-using.html

var worker = new Worker('fileupload.js');

worker.onmessage = function (e) {
    alert(e.data);
}

worker.onerror = werror;

function werror(e)
{
    console.log('ERROR: Line ', e.lineno, ' in ', e.filename, ': ', e.message);
}

function handleFileSelect(evt)
{
    evt.stopPropagation();
    evt.preventDefault();

    var files = evt.dataTransfer.files || evt.target.files;
    // FileList object.

    worker.postMessage({
        'files': files
    });

    //Sending File list to worker
    // files is a FileList of File objects. List some properties.
    var output = [];
    for (var i = 0, f; f = files[i]; i++) {
        output.push('<li><strong>', escape(f.name), '</strong> (', f.type || 'n/a', ') - ',
            f.size, ' bytes, last modified: ', f.lastModifiedDate ? f.lastModifiedDate.toLocaleDateString() : 'n/a', '</li>');
    }

    document.getElementById('list').innerHTML = '<ul>' + output.join('') + '</ul>';
}

function handleDragOver(evt) {
    evt.stopPropagation();
    evt.preventDefault();
    evt.dataTransfer.dropEffect = 'copy'; // Explicitly show this is a copy.
}

// Setup the dnd listeners.
var dropZone = document.getElementById('drop_zone');
dropZone.addEventListener('dragover', handleDragOver, false);
dropZone.addEventListener('drop', handleFileSelect, false);
document.getElementById('files').addEventListener('change', handleFileSelect, false);
