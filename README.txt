
This program sort or merge a list of the folders and files in Visual Studio Family DB Project File (.dbproj).

Sort:
	DbProjectMerge.exe myproject.dbproj

Merge:
	DbProjectMerge.exe old.dbproj new.dbproj
	(Merged project file is based on 'new.dbproj')


Visual Studioでチームで開発しているときに、みんなでいろいろとファイルを追加してると
プロジェクトファイルのファイルリストを正しくマージしていくのが、とても面倒です。

その対策に、このプログラムは、2つの機能を提供します。
1. プロジェクトファイルの中の、ファイルとフォルダのリストをソートして見やすくする機能
2. 2つのプロジェクトファイルの中の、ファイルとフォルダのリストをマージする機能


ぼっち開発でも使えますよ！
ファイルの追加操作した順に並ぶとかありえません。


This program is use "AnonymousComparer - lambda compare selector for Linq"
http://linqcomparer.codeplex.com/
Thanks for neue cc
