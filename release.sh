#! /usr/bin/env bash
set -euo pipefail

script_path="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
vers="$(python3 setup.py --version)"

read -p "Has the version been updated (current: ${vers})? " -n 1 -r
if ! [[ $REPLY =~ ^[Yy]$ ]]; then
    exit 1
fi
echo

image_name="mhill421/plex-media-formatter"
image_tag="$vers"
docker build "$script_path" -t "${image_name}:${image_tag}"
docker tag "${image_name}:${image_tag}" "${image_name}:latest"

docker push "${image_name}:${image_tag}"
docker push "${image_name}:latest"
