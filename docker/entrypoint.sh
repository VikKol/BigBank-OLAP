#!/bin/bash

exec python2 /opt/imply-2.8.7/bin/druid_ingest_data.py -f /tmp/data/ingestion_spec.json &

cd /opt/imply-2.8.7/

# Start Imply/Druid cluster
bin/supervise -c conf/supervise/quickstart.conf