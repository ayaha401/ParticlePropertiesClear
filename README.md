# ParticlePropertiesClear
Unityでパーティクルシステムで使用しないモジュールから参照されているアセットを外すEditor拡張

## 使用しないモジュールから参照されているアセットを外すとは？
![image](https://github.com/user-attachments/assets/97af7c16-d9c3-4052-bf31-673164d2808e)<br>
画像のようにShapeモジュールでShapeをMeshに設定し、Meshを入れた状態でShapeモジュールのチェックボックスを外すorShapeをMesh以外に設定などの行為を行うとUnityの内部的には履歴として設定していたアセット参照を持ったままになっている
この状態だと無駄にアセットをロードするためEditor拡張で使わない機能で設定されているアセットにはNullを入れます。

# 導入方法
スクリプトをEditorのディレクトリに入れる

# 使い方
![image](https://github.com/user-attachments/assets/db84100d-1521-4826-8727-5fc0426bd737)<br>

AyahaGraphicDevelopTools > ParticleUnusedPropertiesClearからウィンドウを出し、`対象Prefabリスト`にパーティクルを入れる
その後`消す`を押すことで使用していないアセットを消すことができる

# 不具合
