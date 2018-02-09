
export const guid = (): string => {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

export const url = (image, prefix) => {
    if (prefix != '' && prefix != undefined)
        prefix += '-';
    else prefix = '';

    const s = `${(window as any).baseUrl}${image.id}/${prefix}${image.fileName}`;

    return s;
}