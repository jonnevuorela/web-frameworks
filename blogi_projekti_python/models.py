from typing import List

from sqlalchemy import (
    Column,
    ForeignKey,
    Index,
    Integer,
    LargeBinary,
    Table,
    Text,
    text,
)
from sqlalchemy.orm import Mapped, declarative_base, mapped_column, relationship

Base = declarative_base()

BlogTag = Table(
    "BlogTag",
    Base.metadata,
    Column(
        "BlogId",
        Integer,
        ForeignKey("Blogs.Id", ondelete="CASCADE"),
        primary_key=True,
        nullable=False,
    ),
    Column(
        "TagId",
        Integer,
        ForeignKey("Tags.Id", ondelete="CASCADE"),
        primary_key=True,
        nullable=False,
    ),
    Index("IX_BlogTag_TagId", "TagId"),
)


class Users(Base):
    __tablename__ = "Users"
    __table_args__ = (Index("IX_Users_UserName", "UserName", unique=True),)

    Id = mapped_column(Integer, primary_key=True)
    HashedPassword = mapped_column(LargeBinary, nullable=False)
    PasswordSalt = mapped_column(LargeBinary, nullable=False)
    Role = mapped_column(Text, nullable=False)
    UserName = mapped_column(Text, nullable=False)
    Xp = mapped_column(Integer, nullable=False, server_default=text("0"))

    Blogs: Mapped[List["Blogs"]] = relationship(
        "Blogs", uselist=True, back_populates="Users_"
    )


class Tags(Base):
    __tablename__ = "Tags"

    Id = mapped_column(Integer, primary_key=True)
    TagText = mapped_column(Text, nullable=False)

    Blogs: Mapped[List["Blogs"]] = relationship(
        "Blogs", secondary=BlogTag, back_populates="Tags_"
    )


class Blogs(Base):
    __tablename__ = "Blogs"
    __table_args__ = (Index("IX_Blogs_AppUserId", "AppUserId"),)

    Id = mapped_column(Integer, primary_key=True)
    Title = mapped_column(Text, nullable=False)
    Content = mapped_column(Text, nullable=False)
    AppUserId = mapped_column(
        ForeignKey("Users.Id", ondelete="CASCADE"), nullable=False
    )

    Users_: Mapped["Users"] = relationship("Users", back_populates="Blogs")
    Tags_: Mapped[List["Tags"]] = relationship(
        "Tags", secondary=BlogTag, back_populates="Blogs"
    )
