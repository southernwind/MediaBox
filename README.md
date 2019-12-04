# MediaBox(仮称)
  
## MediaBoxとは
 
 **概要**  
 MediaBoxはメディアファイルの管理ツールです。  
 
 **特徴**  
 - メディアファイルのメタデータを書き換えたり、ファイルの存在するディレクトリにサムネイルを作成したりしません。元の画像やフォルダには手を加えないため、ファイルが壊れたり、紛失したり、あちこちに余計なファイルが作られたりということがありません。
 - GPS情報のついたメディアファイルはマップ上に表示することができます。
 - ほぼすべての動作を非同期で行っているため、動作中に固まってしまうことがありません。
 - 大量のファイルを管理していても軽快に動作します。

 
**デモ**
 
 マップ表示  
![マップデモ](https://raw.github.com/wiki/southernwind/MediaBox/images/main/map.gif)
  
フィルタリング  
![フィルターデモ](https://raw.github.com/wiki/southernwind/MediaBox/images/main/filter.gif)
 
## 主な機能
 
- フィルタリング、ソート
- ワードやタグによる検索
- マップ表示
- 一覧表示
- フォルダ監視によるメディアファイルの自動登録、変更検知
- 逆ジオコーディングによるGPS座標から住所への変換
- GPS情報のついていない画像にGPS情報を設定し、地図上に表示  
![リバースジオコーディング](https://raw.github.com/wiki/southernwind/MediaBox/images/main/reverse_geocoding.png)
 
## 必要要件
 
- .NET Core 3.1以降

## 使い方
 
1. ツール→設定→スキャン設定を開き、右上の「＋ボタン」を押して管理対象画像のあるフォルダを登録してください。初期設定としてマイピクチャが入っていますが、不要な場合は削除してください。
2. 自動的にフォルダのスキャンが開始されます。
3. メインウィンドウに戻り、左側ペインのフォルダタブを開くとスキャンが完了した画像がツリー形式で表示されます。  
フォルダを開くと中央部にサムネイルが表示されます。
4. 上部のボタンからライブラリ表示、詳細表示、マップ表示の切り替えが行えます。
5. また、右上のテキストボックスからワードによる画像の検索が行えます。