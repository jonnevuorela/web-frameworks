import datetime
import os
import uuid

import jwt
from dotenv import load_dotenv

import models
from custom_exceptions.not_found_exception import NotFoundException
from tools.abc_token_tool import ABCTokenTool

load_dotenv()


class SymmetricTokenTool(ABCTokenTool):
    def create_token(self, user: models.Users) -> str:
        jwt_payload = {
            "sub": str(user.Id),
            "name": user.UserName,
            "jti": str(uuid.uuid4()),
            "role": user.Role,
            "XP": user.Xp,
            "exp": datetime.datetime.now() + datetime.timedelta(days=7),
        }

        _token_key = os.getenv("TOKEN_KEY", None)
        if _token_key is None:
            raise NotFoundException("token not found")

        return jwt.encode(jwt_payload, _token_key, algorithm="HS512")

    def decode_token(self, token: str) -> dict | None:

        _token_key = os.getenv("TOKEN_KEY", None)
        if _token_key is None:
            raise NotFoundException("token not found")

        decoded_payload = jwt.decode(token, _token_key, algorithms="HS512")
        return decoded_payload
