from fastapi import FastAPI

from controllers import blogs_controller, users_controller

app = FastAPI()

app.include_router(users_controller.router)

app.include_router(blogs_controller.router)
