Возведение в степень. 
2 клиен-серверных приложения .Net Core
Запускается сервер и клиент. При подлючении клиента ему высылается инструкция.
1) Клиент может отправить 2 параметра: число (от 1 до 100) и степень (square/cube). 
Если число нечетное, то возведение в степень происходят внетри клиентского приложения, если четное,
то данные отправляются на сервер и червер производит математические действия, после чего высылает ответ обратно.

2) Тоже самое, но числа от 1 до 50, и всегда Cube. Значения всегда высчитываются внутри серверного приложения. 


В приложениях отработано:
-ожидание подключения клиента к серверу;
-многопотоковая работа с серверов;
-передача данных по TCP междук сервером и клиентом.
