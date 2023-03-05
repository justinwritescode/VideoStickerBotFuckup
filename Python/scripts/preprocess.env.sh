#!/bin/bash
echo "#!/bin/bash" > preprocessedd.env.sh
awk 'NF{print "export "$0}' .env >> preprocessedd.env.sh
chmod +x preprocessedd.env.sh
