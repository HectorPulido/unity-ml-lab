"""
Entrypoint for the Llama Server.
"""

import os
import asyncio
from dotenv import load_dotenv
from llama_server import LlamaServer


if __name__ == "__main__":
    load_dotenv()

    HOST = os.getenv("HOST")
    PORT = int(os.getenv("PORT"))
    LLM_MODEL = os.getenv("LLM_MODEL")
    EMBEDDER_MODEL = os.getenv("EMBEDDER_MODEL")

    llama_server = LlamaServer(
        HOST,
        PORT,
        LLM_MODEL,
        EMBEDDER_MODEL,
    )
    asyncio.run(llama_server.main())
