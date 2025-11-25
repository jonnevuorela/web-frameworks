import time
from urllib.request import Request

from starlette.middleware.base import BaseHTTPMiddleware


class LoggingMiddleware(BaseHTTPMiddleware):
    async def dispatch(self, request: Request, call_next):

        start = time.time()
        response = await call_next(request)
        end = time.time()
        print(f"Time it took to run the request: {end-start} seconds")
        return response
