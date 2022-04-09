FROM python:3.9
WORKDIR /var/pmf

# ADD setup.py .
# ADD pmf/_version.py pmf/
# RUN pip install .

# ADD . .

ADD . .
RUN pip install .

ENTRYPOINT [ "pmf", "format" ]
