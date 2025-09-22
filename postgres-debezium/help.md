# Subindo tudo

No diretório do projeto:

docker compose up -d

Verifique a saúde:

docker compose ps

# Registrar o conector no Kafka Connect
curl -X POST http://localhost:8083/connectors \
  -H "Content-Type: application/json" \
  --data @connectors/pg-source.json

Conferir se subiu:

curl -s http://localhost:8083/connectors | jq
curl -s http://localhost:8083/connectors/pg-source/status | jq


Debezium criará (ou usará) o logical replication slot slot_pg_appdb e começará com snapshot da tabela, depois passará a stream de mudanças (CDC).

# Teste rápido (inserir/atualizar/deletar)

Abra um psql na sua máquina (ou use um cliente GUI):

# Inserir
docker exec -it pg16 psql -U app -d appdb -c \
  "INSERT INTO public.customers(name,email) VALUES ('Mateus','mateus@example.com');"

# Atualizar
docker exec -it pg16 psql -U app -d appdb -c \
  "UPDATE public.customers SET name='Mateus Back' WHERE email='mateus@example.com';"

# Deletar (opcional)
docker exec -it pg16 psql -U app -d appdb -c \
  "DELETE FROM public.customers WHERE email='mateus@example.com';"


Consumir o tópico gerado pelo Debezium (prefixo pg → tópico pg.public.customers):

# Consumidor com kcat (dentro do container utilitário)
docker exec -it kcat kcat -b kafka:9092 -t pg.public.customers -C -o beginning


Você verá os eventos JSON (create/update/delete) chegando.

Dicas e ajustes úteis

Vários schemas/tabelas: adicione mais tabelas na publicação e em table.include.list (ex.: public.customers,public.orders).

Autocriar publicação: você pode remover a criação de pub_appdb do SQL e usar no connector:

"publication.autocreate.mode": "filtered" (ou all_tables) e omitir publication.name (ou alinhá-la).

Snapshot:

Para começar sem snapshot (apenas mudanças futuras): "snapshot.mode": "never".

Retenção do slot: slot.drop.on.stop=false garante que parar o Connect não apaga o slot. Se o Connect ficar muito tempo parado, o Postgres pode acumular WAL; monitore pg_replication_slots.

Formatação dos eventos: estamos usando JSON sem schemas (simples de testar). Se quiser Schema Registry/Avro/JSON Schema, aponte KEY_CONVERTER/VALUE_CONVERTER e rode um Schema Registry (Confluent) adicional.

Produção:

Considere múltiplos brokers Kafka.

Ajuste discos/volumes (WAL e /bitnami/kafka).

Backup do Postgres e rotação/retention dos WALs.

Observabilidade: Prometheus/Grafana, Redpanda Console (UI para Kafka), etc.

Se quiser, eu já adiciono uma UI pro Kafka (ex.: Redpanda Console) ou um Schema Registry ao compose. Também posso adaptar para Debezium Server (em vez de Kafka Connect) se você preferir publicar direto em S3, Pulsar, Kinesis, Pub/Sub etc. Quer essa variante?




## Rodando de novo:

docker compose up -d

curl.exe -s http://localhost:8083/connectors
ou
Invoke-WebRequest `
  -Uri "http://localhost:8083/connectors" `
  -Method Post `
  -ContentType "application/json" `
  -InFile "connectors/pg-source.json"


