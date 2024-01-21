import time

from compas_eve import Message
from compas_eve import Publisher
from compas_eve import Subscriber
from compas_eve import Topic
from compas_eve.mqtt import MqttTransport

topic = Topic("/test12/", Message)
server = MqttTransport("broker.hivemq.com")

for i in range(20):
    msg = Message(result=f"Hello robot #{i}")
    print(f"Publishing message: {msg.result}")
    Publisher(topic, transport=server).publish(msg)
    time.sleep(3)