var fileList = [];
var reader;
var myFile;
var fileAsText = "";
var fileName;
var callBackFxn;
var callBackFxnArgs;
var minimumSizeToZip = 500000;
var disAllowedFileTypes = ['zip', 'ppt', 'pptx', 'rar'];
async function uploadButtonClick(sender, functionToRunAfterUload, ...functionArgs){
    callBackFxn = functionToRunAfterUload;
    callBackFxnArgs = functionArgs;
    $('#myFile').trigger('click');
}

function getBase64(file) {
    myFile = file;
    reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        console.log(reader.result);
        fileAsText = reader.result;
    };
    reader.onerror = function (error) {
        console.log('Error: ', error);
    };
}

async function convertToB64(file) {
    return new Promise((resolve) => {
    const reader = new FileReader()
    reader.onloadend = () => resolve(reader.result)
    reader.readAsDataURL(file)
    });
}

async function convertToBase64(file) {
    myFile = file;
    reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        console.log(reader.result);
        fileAsText = reader.result;
        return reader.result;
    };
    reader.onerror = function (error) {
        console.log('Error: ', error);
        return 0;
    };
}

var julia = 1;
var zips = [];
var base64List = [];
var fileNameList = [];
var numDone = 0;
var checkForRepeatsList = [];
var sizeList = [];

async function zipUpFiles (sender, finalFxn, finalFxnArgs) {
    zips = [];
    base64List = [];
    sizeList = [];
    let numFiles = sender.files.length;
    let loop = 0;
    numDone = 0;

    while (loop < numFiles){
        let selectedFile = sender.files[loop];
        let nameWOExt= selectedFile.name.substr(0, selectedFile.name.lastIndexOf('.'));
        let newName = nameWOExt.replace(/-|,|_|\./gi, " ");
        newName = newName + '.' + selectedFile.name.split('.').pop();
        //let newName = new Regex("([!@#$%^&*()]|(?:[.](?![a-z0-9]+$)))", RegexOptions.IgnoreCase).Replace(selectedFile.name, "+");

        if (checkForRepeatsList.includes(newName)){
            alert('file already uploaded once');
            return 0;
        }
        if (newName.includes('-') || selectedFile.name.includes('_')){
            alert('file name cannot contain - or . or _');
            return 0;
        }
        if (disAllowedFileTypes.includes(selectedFile.name.split('.').pop())){
            alert('invalid file type. choose a different file.');
            return 0;
        }
        checkForRepeatsList.push(newName);
        fileNameList.push(newName);
        let new_zip = new JSZip2();

        let examples = new_zip.folder("examples");
        if (newName.endsWith(".doc")){
            btnSave.disabled = true; 
            fileName = newName + "x";
            let form = new FormData();
            form.append("inputFile", selectedFile, "file");
            let apiUrl = "https://api.cloudmersive.com";
            let apiPath = "/convert/doc/to/docx";
            let headers = {
            "Content-Type": "multipart/form-data",
            Apikey: "1d11b99f-91bd-4240-b4ac-6737c64938e9",
            }

            let convertedFile = await axios.post(apiUrl + apiPath, form, {
                headers,
                responseType: "blob"
            });
                let pdfUrl = URL.createObjectURL(convertedFile.data);
                console.log(pdfUrl);
                examples.file(
                    selectedFile.name + "x",
                    convertedFile.data,
                    { binary: true }
                );
                let zippedUpContent = await examples.generateAsync({ type: "blob", compression: "DEFLATE",
                            compressionOptions: {
                                level: 9
                            }
                    });
                await convertZippedFileToB64(zippedUpContent, numFiles, finalFxn, finalFxnArgs);
        }
        else if (selectedFile.name.endsWith(".ppt")){
            btnSave.disabled = true; 
            fileName = newName + "x";
            let form = new FormData();
            form.append("inputFile", selectedFile, "file");
//            let apiUrl = "https://api.cloudmersive.com";
            let apiPath = "/convert/pptx/to/pdf";
            let headers = {
            "Content-Type": "multipart/form-data",
            Apikey: "1d11b99f-91bd-4240-b4ac-6737c64938e9",
            }
            let convertedFile = await axios.post(apiUrl + apiPath, form, {
                headers,
                responseType: "blob"
            });
                    
            let pdfUrl = URL.createObjectURL(convertedFile.data);
            console.log(pdfUrl);
            examples.file(
                newName + ".pdf",
                convertedFile.data,
                { binary: true }
            );
            let zippedUpContent = await examples.generateAsync({ type: "blob", compression: "DEFLATE",
                        compressionOptions: {
                            level: 9
                        }
                });
            await convertZippedFileToB64(zippedUpContent, numFiles, finalFxn, finalFxnArgs);
        } else {
            examples.file(
                newName,
                selectedFile,
                { binary: true }
            );
            let zippedUpContent = await examples.generateAsync({ type: "blob", compression: "DEFLATE",
                        compressionOptions: {
                            level: 9
                        }
                });
            sizeList.push(selectedFile.size);
            if (selectedFile.size > minimumSizeToZip){
                await convertZippedFileToB64(zippedUpContent, numFiles, finalFxn, finalFxnArgs);
            } else {
                await convertZippedFileToB64(selectedFile, numFiles, finalFxn, finalFxnArgs);           
            }
        }
        loop++;
    }
            
};
async function convertZippedFileToB64(content, numberOfFiles, finalFunction, finalFunctionArgs) {
                let fileAsBinary = await convertToB64(content);
                base64List.push(fileAsBinary);
                numDone++;
                if (numDone == numberOfFiles){
                    let zippedFileList = await sendAllFiles(base64List, fileNameList, sizeList);
                    finalFunction(zippedFileList, ...finalFunctionArgs);
                    base64List = [];
                    fileNameList = [];
                }
}
async function sendAllFiles(dataList, nameList, sizeList){
    let loop = 0;
    let savedFileList = [];
    while (loop < dataList.length){
        let name = await sendFileToServer(dataList[loop], nameList[loop], sizeList[loop]);
        savedFileList.push(name);
        loop++;
    }
    return savedFileList;
}
async function sendFileToServer(fileAsBinary, myFileName, size) {
    if (size > minimumSizeToZip){
        myFileName = myFileName + '.zip';
    }

    if (fileAsBinary == ""){
        alert("no file has been selected!");
        return 0;
    }
    chunks = [];
    str = fileAsBinary;
    for (var i = 0, charsLength = str.length; i < charsLength; i += 50000) {
        chunks.push(str.substring(i, i + 50000));
    }
    console.log(chunks);
    chunks[0] = chunks[0].split(",")[1];
    var start = 0;
    var endPointPath = '/WebService/AjaxService.asmx/SaveBytes1';

    while (start < chunks.length) {
        progressBar.value = parseInt(start/chunks.length * 100);

        var resp = await axios.post(endPointPath, { 'file': chunks[start], 'directory':'Documents', 'filename': myFileName, 'loop' : start, 'isFinal' : 'no'},
                {
                    headers: {
                    'content-type': 'application/json'
                    }
                });

        console.log(start);
        start = start + 1;       
    }

    var newFileName = await axios.post(endPointPath, { 'file': 'ok', 'directory':'Documents', 'filename': myFileName, 'loop' : start, 'isFinal' : 'Yes'},
                            {
                                headers: {
                                'content-type': 'application/json'
                                },
                                responseType: 'text'
                            });
    var newFileNameJSON = newFileName.data;
    var newfilenameString = JSON.parse(newFileNameJSON).d;
    console.log('Saved File is ' + JSON.parse(newFileNameJSON).d);

    progressBar.value = 100;

    var newFileNameForList = await axios.post('/WebService/AjaxService.asmx/notifyDone', {'directory':'Documents', 'filename': newfilenameString},
                            {
                                headers: {
                                'content-type': 'application/json'
                                },
                                responseType: 'text'
                            });
    fileAsBinary = "";
    return newfilenameString;
}

var btn;
var span;
var data;
var chunks;
