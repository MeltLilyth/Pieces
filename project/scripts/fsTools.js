const path = require('path');
const url = require('url');
const fs = require('fs');
const xlsx = require('xlsx');
const request = require('request');

//文件操作
var fsTools = function(){
    return this;
}

fsTools.jsonArray = new Array(); //用于存储某个文件夹下的所有的json文件路径
fsTools.languageArray = new Array(); //用于存储中文文本

//图片已测试,json,bin等其他资源格式未验证
fsTools.downloadFile = function(url,savePath){
    var fileName = url.split("/")[url.split("/").length -1];
    savePath = path.join(savePath,fileName);
    var writeStream = fs.createWriteStream(savePath);
    var readStream = request(url);
    readStream.pipe(writeStream);

    readStream.on('error',function(err){
        err && console.log(err);
    });

    readStream.on('finish',function(){
        console.log("download end, savePath:  "+ savePath);
    });
}   

fsTools.getAllJsonFiles = function(targetPath){
    var files = fs.readdirSync(targetPath);
    files.forEach(value=>{
       fs.lstatSync(path.join(targetPath,value)).isDirectory()? fsTools.jsonArray.concat(fsTools.getAllJsonFiles(path.join(targetPath,value))) 
        : fsTools.jsonArray.push(path.join(targetPath,value));   
    });

    return fsTools.jsonArray;
}

//用于获取特定格式的json中文文本
fsTools.getAllJsonLanguage = function(arr){
    var getJsonLanguage = function(arr){
        arr.forEach(value=>{
            value.length > 0? getJsonLanguage(value) : 
                (value._component == undefined? getJsonLanguage(value._component) : (function(item){
                    if(item.__type__ == "cc.Label"){
                        item._fontSize == undefined ? fsTools.languageArray.push(item._string) : fsTools.languageArray.push([item._string,item._fontSize]);
                    }
                    else if(item.__type__ == "cc.RichText"){
                        item._fontSize == undefined ? fsTools.languageArray.push(item._N$string) : fsTools.languageArray.push([item._N$string,item._fontSize]);
                    }
                })(value));
        });
    }

    arr.forEach(value=>{
        var data = JSON.parse(fs.readFileSync(value).toString());
        getJsonLanguage(data);    
    });

    return fsTools.languageArray;
}//基于在公司的写法基础上改进的，没有测试用例所以无法确定代码能否获得自己想要的效果

//将获取到的json文本导出为excel文件
fsTools.exportExcel = function(arr){
    var dataArray = new Array();
    arr.forEach(value=>{
        let data = {};
        //key 中文简体文本 中文繁体文本 英文文本 是否自定义色值 色值 是否自定义中文字体大小 中文字体大小 是否自定义英文字体大小 英文字体大小 是否使用系统字体 分割字符
        data.key = "";
        data.中文简体 = typeof value == 'object'? value[0] : value;
        data.中文繁体 = "NULL";
        data.英文文本 = "";
        data.是否自定义色值 = false;
        data.色值 = "255_255_255_255";
        data.是否自定义中文字体大小 = false;
        data.中文字体大小 = typeof value == 'object'? value[1] : 10;
        data.是否自定义英文字体大小 = false;
        data.英文字体大小 = typeof value == 'object' && value[1] > 20? value[1] - 4 : 10;
        data.是否使用系统字体 = false;
        data.分割字符 = "NULL";

        dataArray.push(data);
    });
    var excelData = {
        SheetNames: ['Sheet1'],
        Sheets: {
          'Sheet1': xlsx.utils.json_to_sheet(dataArray)            
        } 
    }
    
    xlsx.writeFile(excelData,"./Book1.xlsx");
}

//读取excel文本内容并导出为json文件
fsTools.exportJsonFile = function(excelPath){
    var data = xlsx.readFile(excelPath);
    var jsonDataArray = new Array();
    xlsx.utils.sheet_to_json(data.Sheets[0]).forEach(value=>{
        let data = {};

        data.jsonKey = value.key;
        data.dataContent = {};
        data.dataContent.chinese_simple = value.中文简体;
        data.dataContent.chinese_tradition = value.中文繁体;
        data.dataContent.english = value.英文文本;
        data.dataContent.isColored = value.是否自定义色值;
        data.dataContent.color = value.色值.split("_");
        data.dataContent.isUseSimplesize = value.是否自定义中文字体大小;
        dataContent.simpleSize = value.中文字体大小;
        data.dataContent.isUseTradsize = value.是否自定义中文字体大小;
        data.dataContent.tradSize = value.中文字体大小;
        data.dataContent.isUseEnglishsize = value.是否自定义英文字体大小;
        data.dataContent.englishSize = value.英文字体大小;
        data.dataContent.isUsesysfont = value.是否使用系统字体;
        value.分割字符? data.dataContent.params = value.分割字符 : null;
        jsonDataArray.push(data);
    });

    fs.writeFileSync("./language.json",JSON.stringify(jsonDataArray));
}
