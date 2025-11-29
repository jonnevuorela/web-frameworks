from fastapi import FastAPI

from controllers import blogs_controller, categorys_controller, users_controller
from tools import loggin_middleware

app = FastAPI()

app.add_middleware(loggin_middleware.LoggingMiddleware)

app.include_router(users_controller.router)

app.include_router(blogs_controller.router)

app.include_router(categorys_controller.router)
