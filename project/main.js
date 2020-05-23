const {app, BrowserWindow} = require('electron');
const path = require('path');

function createWindow(){
    let mainWindow = new BrowserWindow({
        width: 1000,
        height: 600,
        webPreferences:{
            preload: path.join(__dirname,"preload.js"),
            nodeIntegration:true //这一句非常重要,保证你的脚本能引用到nodejs库[前提是你在项目中有安装，代码中能require到]
        },
    });

    mainWindow.loadFile("./view/index.html");
}

app.whenReady().then(()=>{
    createWindow();

    app.on('activate',()=>{
        if(BrowserWindow.getAllWindows().length === 0){
            createWindow();
        }
    })
});

app.on('window-all-closed',()=>{
    if(process.platform !== 'drawin')app.quit();
});

