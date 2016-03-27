var myApp=angular.module("myApp",["ngResource","ngMessages","ui.bootstrap"]);myApp.value("flg","sssssss"),myApp.service("CanvasService",["flg",function(a){var b=new createjs.Bitmap("./img/nami.jpg"),c=new createjs.Bitmap("./img/koi.jpg"),d=new createjs.Bitmap("./img/shisho.png"),e=new createjs.Bitmap("./img/matsuo_basyo.png"),f=new createjs.Stage("test-canvas");createjs.Ticker.setFPS(60),createjs.Ticker.addEventListener("tick",function(){f.update()});var g=500,h=550;this.LoadImgs=function(a){var c=new createjs.LoadQueue;c.loadFile({id:"backImgNami",src:"./img/nami.jpg"}),c.load(),c.addEventListener("complete",function(){a=new createjs.Bitmap(c.getResult("backImgNami")),b=new createjs.Bitmap(c.getResult("backImgNami"))})},this.PageInitCanvas=function(){var a=new createjs.Shape;a.graphics.beginLinearGradientFill(["#ffffe0","#32cd32"],[.1,1],250,0,250,500).drawRect(0,0,500,600),f.addChild(a);var b=new createjs.Text("初期化中…","bold 40px serif","#000000");b.textAlign="center",b.x=g/2,b.y=h/2,createjs.Tween.get(b,{loop:!0}).to({alpha:0},2e3).to({alpha:1},2e3),f.addChild(b),f.update()},this.MakeBack=function(a){var d,e=new createjs.Shape;d="0"==a?b:c,d.x=0,d.y=0,f.addChild(d),e.graphics.beginFill("#f8f8ff").drawRect(100,20,g-200,h-40),f.addChild(e),f.update()};var i,j,k,l;this.StartLoadingAnim=function(a){"0"==a?(l=e,l.x=-10,l.y=100):(l=d,l.x=-60,l.y=100),i=new createjs.Text("思","bold 50px serif","#FF0000"),i.x=g/2-70,i.y=80,i.textAlign="center",i.textBaseline="middle",j=new createjs.Text("案","bold 50px serif","#FF0000"),j.x=g/2,j.y=80,j.textAlign="center",j.textBaseline="middle",k=new createjs.Text("中","bold 50px serif","#FF0000"),k.x=g/2+70,k.y=80,k.textAlign="center",k.textBaseline="middle",f.addChild(l),f.addChild(i),f.addChild(j),f.addChild(k),f.update(),createjs.Tween.get(i).to({rotation:360},2e3),createjs.Tween.get(j).wait(500).to({rotation:360},2e3),createjs.Tween.get(k).wait(500).to({rotation:360},2e3),f.update()},this.ShowHaiku=function(a,b){f.removeChild(i),f.removeChild(j),f.removeChild(k),f.removeChild(l);for(var c=0,d=0;d<a.length;d++){for(var e=a[d].split(""),g=b[d][1],h=0;h<e.length;h++){var m=new createjs.Text(e[h].replace("ー","｜"),"bold 40px serif","#000000");m.x=b[d][0],m.y=g,m.textBaseline="middle",m.textAlign="center",m.alpha=0,f.addChild(m),g+=50,createjs.Tween.get(m).wait(c).to({alpha:1},2e3),c+=500}c+=200}var n=new createjs.Shape;n.graphics.beginFill("yellow").drawCircle(0,0,50),n.y=50,n.x=50,f.addChild(n),f.update(),createjs.Tween.get(n).to({x:700,y:700},2500)}}]),myApp.controller("mainCtrl",["$scope","$resource","$modal","CanvasService","flg",function(a,b,c,d,e){a.initTxt="ある日の事でございます。御釈迦様は極楽の蓮池のふちを、独りでぶらぶら御歩きになっていらっしゃいました。池の中に咲いている蓮の花は、みんな玉のようにまっ白で、そのまん中にある金色の蕊からは、何とも云えない好い匂いが、絶間なくあたりへ溢れて居ります。極楽は丁度朝なのでございましょう。やがて御釈迦様はその池のふちに御佇みになって、水の面を蔽っている蓮の葉の間から、ふと下の様子を御覧になりました。この極楽の蓮池の下は、丁度地獄の底に当って居りますから、水晶のような水を透き徹して、三途の河や針の山の景色が、丁度覗き眼鏡を見るように、はっきりと見えるのでございます。",a.onsubmit=function(){a.isLoading=!0;var c=a.postedData.mode;d.MakeBack(c),d.StartLoadingAnim(c);var e=b("RisshiSummary/makeHaiku");a.HaikuResult=e.save(a.postedData,function(b){var e=[b.Part1,b.Part2,b.Part3];"1"==c&&e.push(b.Part4);var f="0"==c?[[320,60],[250,140],[180,270]]:[[340,60],[280,60],[220,60],[160,270]];d.ShowHaiku(e,f),a.isLoading=!1},function(){alert("通信エラー：しばらくしてからもう一度アクセスしてください。"),a.isLoading=!1})},a.reset=function(){a.postedData.inputedTxt=""},a.changeMode=function(){"1"==a.postedData.mode?a.postedData.mode="0":a.postedData.mode="1"};var f;a.openWikiWindow=function(){a.wikiKeyword={},f=c.open({templateUrl:"W_Wiki",scope:a})},a.getWiki=function(){if(!a.wikiKeyword.inp||""==a.wikiKeyword.inp)return alert("何かキーワードを入力してください");var c=b("RisshiSummary/getWiki/:keyword");c.get({keyword:a.wikiKeyword.inp},function(b){a.postedData={inputedTxt:b.WikiContent,mode:a.postedData.mode},f.close()},function(){alert("通信エラー：しばらくしてからもう一度アクセスしてください。")})},a.init=function(){a.isLoading=!0,d.PageInitCanvas();setTimeout(function(){a.isLoading=!1,a.onsubmit()},2e3)}}]),myApp.run(["$window","CanvasService",function(a,b){}]);