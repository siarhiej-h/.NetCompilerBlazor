const monacoInterop = {};

monacoInterop.editors = {};

monacoInterop.initialize = (elementId, initialCode, language) => {
    require.config({ paths: { 'vs': 'monaco-editor/min/vs' } });
    require(['vs/editor/editor.main'], () => {
        const editor = monaco.editor.create(document.getElementById(elementId), {
            value: initialCode,
            language: language
        });
        monacoInterop.editors[elementId] = editor;
    });
};

monacoInterop.getCode = (elementId) => monacoInterop.editors[elementId].getValue();

monacoInterop.setCode = (elementId, code) => monacoInterop.editors[elementId].setValue(code);

monacoInterop.setMarkers = (elementId, markers) => {
    const editor = monacoInterop.editors[elementId];
    const model = editor.getModel();
    monaco.editor.setModelMarkers(model, null, markers);
};

window.monacoInterop = monacoInterop;