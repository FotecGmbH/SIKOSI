window.Alert = function(message) {
    alert(message);
    return true;
}

window.Confirm = function(message) {
    return confirm(message);
}

window.SaveAs = function (filename, fileContent) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + encodeURIComponent(fileContent)
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

window.setFileInputLabel = (id, text) => {
    var elem = document.getElementById(id);
    if (elem) {
        var parent = elem.parentElement;
        var label = parent.querySelector("label");
        if (label) {
            label.textContent = text;
        }
    }
}