# syntax=docker/dockerfile:experimental

FROM python:3-bullseye as base

SHELL [ "powershell", "-command"]

RUN mkdir /scripts
RUN --mount 'type=volume,source=$(pwd)/scripts,dst=/scripts,readonly=false,volume-driver=local'
RUN /scripts/preprocessseed.env.sh;
RUN --mount type=volume,source=VirtualStickerBoyEnv,target=/build/venv,readwrite=true
RUN --mount type=bind,source=$(pwd)/database,target=/build/dtabase,from=host,readwrite=true,sharing=shared

# DCKER CP ./** ./

# - name: Create and start virtual environment


# RUN python -m pip install --upgrade pip
# RUN  pip install pillow
# RUN pip install psycopg2-binary
# RUN apt install -y 
# RUN pip install freeze -r requirements.txt > requirements-new.txt
# RUN grep -v -x -f requirements.txt requirements-new.txt > requirements-final.txt
# RUN echo "Installing the following dependencies:"
# RUN cat requirements-final.txt
# RUN pip install --no-deps -r requirements-final.txt -t py_lib
# RUN RUN pip imstall -r requirements.txt
