
from fastapi import Depends
from sqlalchemy.orm import Session

from db import get_db
from services.user_service import UserService


def user_service_factory(repository: Session = Depends(get_db)):
    return UserService(repository)
