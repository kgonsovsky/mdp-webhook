echo "czookeeper"
systemctl is-active czookeeper | cat
echo ""

echo "ckafka"
systemctl is-active ckafka | cat
echo ""

echo "cschema"
systemctl is-active cschema | cat
echo ""

echo "crest"
systemctl is-active crest | cat
echo ""

echo "cconnect"
systemctl is-active cconnect | cat
echo ""

echo "ccenter"
systemctl is-active ccenter | cat