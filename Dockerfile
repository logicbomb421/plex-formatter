FROM python:3.9-alpine
WORKDIR /var/pmf

ADD . .
RUN pip install .

ENTRYPOINT [ "pmf", "format" ]
