unlink /crun/file
keytool -printcert -sslserver mdpsinkacceptorfunction20220620204529.azurewebsites.net:443 -rfc | keytool -import -noprompt -alias nm -keystore /crun/file -storepass changeit
confluent local services start
confluent local services connect connector load AzureSink --config /crun/azure-sink.json
confluent local services connect connector load HttpSink --config /crun/http-sink.json