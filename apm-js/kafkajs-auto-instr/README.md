Optional read for setting up Kafka: https://www.digitalocean.com/community/developer-center/how-to-deploy-kafka-on-docker-and-digitalocean-kubernetes

- Start otelcol in Docker `docker run --rm -e SPLUNK_ACCESS_TOKEN=<SPLUNK_ACCESS_TOKEN> -e SPLUNK_REALM=<SPLUNK_REAM> \
-p 13133:13133 -p 14250:14250 -p 14268:14268 -p 4317:4317 -p 4318:4318 -p 6060:6060 \
-p 7276:7276 -p 8888:8888 -p 9080:9080 -p 9411:9411 -p 9943:9943 \
--name otelcol quay.io/signalfx/splunk-otel-collector:latest`
- run `docker-compose up`
- Test that the ports are working `nc -zv localhost 22181`
- Test that the ports are working `nc -zv localhost 29092`
  - Source courtesy https://www.baeldung.com/ops/kafka-docker-setup
- `npm run consumer`
- `npm run producer`
