"""
Llama server package
"""
import json
import asyncio
import websockets
from gpt4all import GPT4All
from translator import Translator
from sentence_transformers import SentenceTransformer, util


class LlamaServer:
    """
    Websocket server that receives a text and returns a generated text
    """

    def __init__(self, host, port, llm_model, embedder_model):
        self.host = host
        self.port = port
        self.model = GPT4All(llm_model)
        self.translator = Translator()
        self.sentence_model = SentenceTransformer(embedder_model)

        self.event_handlers = {
            "generate": self.handle_generate_text,
            "translate": self.handle_translate_text,
            "embed": self.handle_embed_text,
        }

        self.model_generation_lock = False

    async def handle_embed_text(self, **kwargs):
        query = kwargs.pop("query")
        passage = kwargs.pop("sentences")

        query_embedding = self.sentence_model.encode(query)
        passage_embedding = self.sentence_model.encode(passage)

        return util.dot_score(query_embedding, passage_embedding)

    async def handle_translate_text(self, **kwargs):
        query = kwargs.pop("query")
        language = kwargs.pop("language")

        if language == "en_to_es":
            return self.translator.english_to_spanish(query)

        if language == "es_to_en":
            return self.translator.spanish_to_english(query)

        return query

    async def handle_generate_text(self, **kwargs):
        while self.model_generation_lock:
            await asyncio.sleep(10)
        text = kwargs.pop("text")
        self.model_generation_lock = True
        generated_text = self.model.generate(text, **kwargs)
        self.model_generation_lock = False
        return generated_text

    async def handle_event(self, event):
        event = json.loads(event)
        event_type = event.pop("event_type")

        if not event_type:
            return "Event type not found"

        if event_type not in self.event_handlers:
            return f"Event type {event_type} not supported"

        return await self.event_handlers[event_type](**event)

    async def handle_client(self, websocket, _):
        """
        Manage the connection with the client and the model
        """
        print("Conection stablished")
        try:
            async for event in websocket:
                print(f"Event getted: {event}")
                response = await self.handle_event(event)
                await websocket.send(response)
                print(f"Response send: {response}")
        except websockets.exceptions.ConnectionClosedOK:
            print("Conection closed")

    async def main(self):
        """
        Create the server and wait for connections
        """
        server = await websockets.serve(self.handle_client, self.host, self.port)
        print(f"Listen in: {self.host}:{self.port}")
        await server.wait_closed()
