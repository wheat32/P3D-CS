#!/bin/sh
export LD_LIBRARY_PATH="/app/lib/pokemon3d:$LD_LIBRARY_PATH"
exec /app/lib/pokemon3d/Pokemon3D "$@"
