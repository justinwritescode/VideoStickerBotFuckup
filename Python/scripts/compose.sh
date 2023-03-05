#!/bin/bash

python -m venv ${APP_ROOT}/venv
source ${APP_ROOT}/venv/bin/activate
python -m bot
echo "Getting all the dependencies of the requirements..."
pip freeze -r ${BUILD_DIR}/requirements.txt > ${BUILD_DIR}/requirements-new.txt
echo "Calculating the difference..."
grep -v -x -f ${BUILD_DIR}/requirements-new.txt ${BUILD_DIR}/requirements.txt 
echo "Adding back the ones we want..."
cat ${BUILD_DIR}/requirements.txt >> ${BUILD_DIR}/requirements-new.txt
# cat ${BUILD_DIR}/requirements-new.txt | grep -vE "$(cat ${BUILD_DIR}/unrequirements.txt)" > ${BUILD_DIR}/requirements-final.txt
cat ${BUILD_DIR}/requirements-final.txt | grep -vE "$(cat ${BUILD_DIR}/no-options-requirements.txt)" > ${BUILD_DIR}/requirements-final.txt
cat ${BUILD_DIR}/requirements-always-require.txt >> ${BUILD_DIR}/requirements-final.txt
#awk 'NF{print $0 " --install-option=\"--no-deps\" --install-option=\"--only-binary\""}' requirements-final.txt > requirements.txt
mv ${BUILD_DIR}/requirements-final.txt ${BUILD_DIR}/requirements.txt
echo "Installing these requirements:"
cat ${BUILD_DIR}/requirements.txt
