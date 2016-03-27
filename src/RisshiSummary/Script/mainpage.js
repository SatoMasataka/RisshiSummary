
var myApp = angular.module('myApp', ["ngResource", "ngMessages", "ui.bootstrap"]);
myApp.value('flg', "sssssss");

myApp.service('CanvasService', ["flg", function (flg) {
    
    //画像：ここで呼ばないとロード間に合わない
    var backImgNami= new createjs.Bitmap("./img/nami.jpg"); //波背景　
    var backImgCarp = new createjs.Bitmap("./img/koi.jpg"); // 鯉背景
    var mainImgShisho = new createjs.Bitmap("./img/shisho.png");
    var mainImgMatsuo = new createjs.Bitmap("./img/matsuo_basyo.png");

    var stage = new createjs.Stage("test-canvas");
    createjs.Ticker.setFPS(60);
    createjs.Ticker.addEventListener('tick', function () { stage.update(); });

    var CanvasW = 500;
    var CanvasH = 550;

   

    this.LoadImgs = function (ret) {
        //画像読み込み
        var queue = new createjs.LoadQueue();
        queue.loadFile({ id: "backImgNami", src: "./img/nami.jpg" });
        queue.load();
        queue.addEventListener('complete', function () {
            ret = new createjs.Bitmap(queue.getResult('backImgNami'));
            backImgNami = new createjs.Bitmap(queue.getResult('backImgNami'));
        });
    }

    ////////////////
    //ページ表示直後
    ///////////////
    this.PageInitCanvas = function(){
        ////背景
        var initback = new createjs.Shape();
        initback.graphics.beginLinearGradientFill(["#ffffe0", "#32cd32"], [0.1, 1.0], 250, 0, 250, 500).drawRect(0, 0, 500, 600);
        stage.addChild(initback);

        

        //「初期化中」
        var shokikaMes = new createjs.Text("初期化中…", "bold 40px serif", "#000000");
        shokikaMes.textAlign = "center";
        shokikaMes.x = CanvasW  / 2  ;
        shokikaMes.y = CanvasH / 2 ;
        createjs.Tween.get(shokikaMes, { loop: true }).to({ alpha: 0 }, 2000).to({ alpha: 1}, 2000);
        stage.addChild(shokikaMes);

        stage.update();
     
    }

    ////////////////////
    //背景＋短冊をセット
    ///////////////////
    this.MakeBack = function (mode) {
        var back = new createjs.Shape();
        var backImg;
        if(mode=="0")
            backImg = backImgNami;
        else
            backImg = backImgCarp;

        backImg.x = 0;
        backImg.y = 0;
        stage.addChild(backImg);
        back.graphics.beginFill("#f8f8ff").drawRect(100, 20, CanvasW-200, CanvasH - 40);　//短冊
        stage.addChild(back);
        stage.update();
    }

    ///////////////
    //ローディング
    ////////////
    var loadingMesShi; //思案中
    var loadingMesAn;
    var loadingMesChu;
    var loadingMainImg;//画像

    this.StartLoadingAnim = function (mode) {
        if (mode == "0") {
            loadingMainImg = mainImgMatsuo;
            loadingMainImg.x = -10;
            loadingMainImg.y = 100;
        } else {
            loadingMainImg = mainImgShisho;
            loadingMainImg.x = -60;
            loadingMainImg.y = 100;
        }

        loadingMesShi = new createjs.Text("思", "bold 50px serif", "#FF0000");
        loadingMesShi.x = CanvasW / 2 - 70;
        loadingMesShi.y = 80;
        loadingMesShi.textAlign = "center";
        loadingMesShi.textBaseline = "middle";

        loadingMesAn = new createjs.Text("案", "bold 50px serif", "#FF0000");
        loadingMesAn.x = CanvasW/2;
        loadingMesAn.y = 80;
        loadingMesAn.textAlign = "center";
        loadingMesAn.textBaseline = "middle";

        loadingMesChu = new createjs.Text("中", "bold 50px serif", "#FF0000");
        loadingMesChu.x = CanvasW / 2 + 70;
        loadingMesChu.y = 80;
        loadingMesChu.textAlign = "center";
        loadingMesChu.textBaseline = "middle";

        stage.addChild(loadingMainImg);
        stage.addChild(loadingMesShi);
        stage.addChild(loadingMesAn);
        stage.addChild(loadingMesChu);
        stage.update();

        createjs.Tween.get(loadingMesShi).to({ /*x: Math.random() * 30, y: Math.random() * 30,*/ rotation: 360 }, 2000);
        createjs.Tween.get(loadingMesAn).wait(500).to({  rotation: 360 }, 2000);
        createjs.Tween.get(loadingMesChu).wait(500).to({ rotation: 360 }, 2000);
        stage.update();
    }

    //////////
    //俳句表示
    //////////
    this.ShowHaiku = function (haikuParts, headPosition) {
        stage.removeChild(loadingMesShi);
        stage.removeChild(loadingMesAn);
        stage.removeChild(loadingMesChu);
        stage.removeChild(loadingMainImg);

        var timelug = 0;
        for (var i = 0; i < haikuParts.length ; i++) {
            var chares = haikuParts[i].split("");　 //縦書きにするので一文字ずつ配列へ
            var locationY = headPosition[i][1];
            for (var j = 0 ; j < chares.length ; j++) {

                var text = new createjs.Text(chares[j].replace("ー", "｜"), "bold 40px serif", "#000000");
                text.x = headPosition[i][0];
                text.y = locationY;
                text.textBaseline = "middle"
                text.textAlign = "center";
                text.alpha = 0;
                stage.addChild(text);
                locationY += 50;
                createjs.Tween.get(text).wait(timelug).to({ alpha: 1 }, 2000);
                timelug += 500;
            }
            timelug += 200;
        }

        var circle = new createjs.Shape();
        circle.graphics.beginFill('yellow').drawCircle(0, 0, 50);//丸
        circle.y = 50;
        circle.x = 50;
        stage.addChild(circle);
        stage.update();
        createjs.Tween.get(circle).to({ x: 700, y: 700 }, 2500);
    }

}]);


myApp.controller('mainCtrl', ['$scope', '$resource', '$modal', 'CanvasService',"flg", function ($scope, $resource, $modal, CanvasService,flg) {

    $scope.initTxt = "ある日の事でございます。御釈迦様は極楽の蓮池のふちを、独りでぶらぶら御歩きになっていらっしゃいました。"+
        "池の中に咲いている蓮の花は、みんな玉のようにまっ白で、そのまん中にある金色の蕊からは、何とも云えない好い匂いが、絶間なくあたりへ溢れて居ります。"+
        "極楽は丁度朝なのでございましょう。やがて御釈迦様はその池のふちに御佇みになって、水の面を蔽っている蓮の葉の間から、" +
        "ふと下の様子を御覧になりました。この極楽の蓮池の下は、丁度地獄の底に当って居りますから、水晶のような水を透き徹して、" +
        "三途の河や針の山の景色が、丁度覗き眼鏡を見るように、はっきりと見えるのでございます。";

    //俳句生成
    $scope.onsubmit = function () {
        $scope.isLoading = true;
        var mode = $scope.postedData.mode
        CanvasService.MakeBack(mode);
        CanvasService.StartLoadingAnim(mode);
       
        //サーバーへSave
        var api2 = $resource('RisshiSummary/makeHaiku');
        $scope.HaikuResult = api2.save($scope.postedData, function (p) {
            ////Canvasへの描画処理////
            var haikuParts = [p.Part1, p.Part2, p.Part3];//各句
            if (mode == "1") { haikuParts.push(p.Part4) };

            var headPosition = (mode == "0") ?
                        [[320, 60], [250, 140], [180, 270]] : [[340, 60], [280, 60], [220, 60], [160, 270]];　//各句開始位置
            
            CanvasService.ShowHaiku(haikuParts, headPosition);
            $scope.isLoading = false;
        }, function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); $scope.isLoading = false; });
    }
    
    //リセット押下
    $scope.reset = function () { $scope.postedData.inputedTxt = ''; }

    //モード変更押下
    $scope.changeMode = function () {
        if ($scope.postedData.mode == "1") { $scope.postedData.mode = "0"; }
        else { $scope.postedData.mode = "1"; }
    }
    
    //wikiから生成押下
    var wikiwindow;
    $scope.openWikiWindow = function () {
        $scope.wikiKeyword = {};

        //モーダル開く
        wikiwindow = $modal.open({
            templateUrl: "W_Wiki",
            scope: $scope
        });
    }

    //ウィンドウ内：wikiから取得押下
    $scope.getWiki = function () {
      
        if (!$scope.wikiKeyword.inp || $scope.wikiKeyword.inp == "") { return alert("何かキーワードを入力してください"); }
        var apiw = $resource("RisshiSummary/getWiki/:keyword");
        apiw.get({keyword: $scope.wikiKeyword.inp }, function (p) {
            $scope.postedData = { 'inputedTxt': p.WikiContent, 'mode': $scope.postedData.mode };
            wikiwindow.close();
            },
            function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); })
    }

    //ロード直後
    $scope.init = function () {
        $scope.isLoading = true;
        CanvasService.PageInitCanvas();
        //画像ロードまでの時間稼ぎ
        var id = setTimeout(
            function () {
                $scope.isLoading = false;
                $scope.onsubmit();   
            }
            ,2000);
    } 
}]);

myApp.run(['$window', 'CanvasService',function ($window,CanvasService) {
    //ここにonload時にさせたい処理
    //CanvasService.PageInitCanvas();
}]);


