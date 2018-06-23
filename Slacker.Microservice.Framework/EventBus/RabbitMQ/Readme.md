Setup Rabbit MQ
1. Install Docker for windows (you need windows 10 pro to make life easier)
2. At a PowerShell command prompt: docker run -d --hostname my-rabbit --name some-rabbit -p 8080:15672 -p 5672:5672 rabbitmq:3-management

	