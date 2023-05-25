# YXReaderAPI
提供怡心阅读的WebAPI

## API使用规范

###  用户注册
|选项|内容|
| ---- | ---- |
Location        |/api/User/Register
Method          |Post
Parameters      |无
Request body    |{<br>"userID": 0,<br>"userName": "string",<br>"password": "string",<br>"email": "string",<br>"personalInfo": "string"<br>}
Responses       |Success：附带赋值了userID的json<br>Confilct：用户名已经被注册了
其他            |请求的userID随意赋值，成功后返回的json会附带用户的userID


