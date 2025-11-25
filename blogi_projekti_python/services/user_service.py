import argon2
import dotenv
from sqlalchemy.orm import Query, Session

import models
from custom_exceptions.not_found_exception import NotFoundException
from services.abc_user_service import ABCUserService
from tools.abc_token_tool import ABCTokenTool


class UserService(ABCUserService):
    def __init__(self, _repository: Session, _token_tool: ABCTokenTool):
        self._repository = _repository
        self._token_tool = _token_tool

    dotenv.load_dotenv()

    def register(self, username: str, password: str) -> models.Users:
        # tästä metodista puuttuu käyttäjän lisäys vielä
        # koska tarvitsemme siihen self._repositorya.

        # hashatty salasana
        hashed_password = argon2.hash_password(password.encode("utf-8"))

        # erillistä password saltia ei tarvita, koska passlib ja argon2 luovat ne itse
        user = models.Users(
            HashedPassword=hashed_password,
            UserName=username,
            PasswordSalt="".encode("utf-8"),
            Role="user",
        )
        self._repository.add(user)
        self._repository.commit()

        return user

    def get_user_by_username(self, username: str) -> models.Users | None:
        user = self._repository.query(models.Users).filter_by(UserName=username).first()
        return user

    def login(self, username: str, password: str) -> str:
        user = self.get_user_by_username(username)
        if user is None:
            raise NotFoundException("user not found")

        if not argon2.verify_password(user.HashedPassword, password.encode("utf-8")):
            raise NotFoundException("user not found")

        return self._token_tool.create_token(user)

    def get_all(self) -> list[models.Users]:
        # tämä rakentaa queryn
        users_query: Query = self._repository.query(models.Users)
        # all()-metodin kutsu suorittaa sen
        users: list[models.Users] = users_query.all()
        return users

    def get_user_by_id(self, _id):
        return self._repository.query(models.Users).filter_by(Id=_id).first()

    def get_user_by_access_token(self, access_token: str) -> models.Users | None:
        decoded_payload = self._token_tool.decode_token(access_token)
        user = self.get_user_by_id(decoded_payload["sub"])
        return user
