window.BlazorDownloadFile = (fileName, contentType, content) => {
    const link = document.createElement('a');
    link.download = fileName;

    const blob = new Blob([new Uint8Array(content)], { type: contentType });
    link.href = window.URL.createObjectURL(blob);
    link.click();

    window.URL.revokeObjectURL(link.href);
};
