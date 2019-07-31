docker kill $(docker ps -q)
docker-compose -p Serilog.Sinks.RabbitMq.Sample -f docker-compose.yaml up -d