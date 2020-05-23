window.languageMap = new Map();

var LanguageTools = (function(){
    function LanguageTools(){
        this.initMap().then((data)=>{
            data = JSON.parse(data);
            data.forEach(value => {
                window.languageMap.set(value.jsonKey,value.dataContent);
            });
        }).catch(()=>{
            console.log("init Map error");
        })
    }

    LanguageTools.prototype.initMap = function(){
        return new Promise((resolve,reject)=>{
            let xhr = new XMLHttpRequest();
            xhr.open("GET","res/confusedRes/language.json");
            xhr.onload = function(){
                if(this.readyState == 4){
                    resolve && resolve(this.responseText);
                }
            }
            xhr.send();
        });    
    }

    LanguageTools.prototype.getLangugeContent = function(jsonKey){
        if(window.languageMap == undefined || window.languageMap == null){
            console.error("init Map Faild");
            return;
        }
        if(window.languageMap.has(jsonKey)){
            return window.languageMap.get(jsonKey);
        }
        else{
            //特殊处理
        }
    }

    return LanguageTools;
}());

window.languageTools = new LanguageTools();