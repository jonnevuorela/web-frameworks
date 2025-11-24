
from sqlalchemy.orm import Session

import argon2
import models
from services.abc_user_service import ABCUserService


class UserService(ABCUserService):

    def __init__(self, _repository: Session):
        self._repository = _repository

    def register(self, username: str, password: str) -> models.Users:
        # tästä metodista puuttuu käyttäjän lisäys vielä
        # koska tarvitsemme siihen self._repositorya.

        # hashatty salasana
        hashed_password = argon2.hash_password(password.encode('utf-8'))

        # erillistä password saltia ei tarvita, koska passlib ja argon2 luovat ne itse
        user = models.Users(HashedPassword=hashed_password, UserName=username, PasswordSalt="".encode('utf-8'), Role="user")
        self._repository.add(user)
        self._repository.commit()

        return user
