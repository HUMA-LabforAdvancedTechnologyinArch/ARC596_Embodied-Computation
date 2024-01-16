import time

from compas_eve import Message
from compas_eve import Publisher
from compas_eve import Subscriber
from compas_eve import Topic
from compas_eve.mqtt import MqttTransport

topic = Topic("/ARC596_test_topic_here/", Message)
server = MqttTransport("broker.hivemq.com")

subcriber = Subscriber(topic, callback=lambda msg: print(f"Received message: {msg}"), transport=server)
subcriber.subscribe()

print("Waiting for messages, press CTRL+C to cancel")

while True:
    time.sleep(1)

for i in range(20):
    msg = Message(text=f"Hello robot #{i}")
    print(f"Publishing message: {msg.text}")
    Publisher(topic, transport=server).publish(msg)
    time.sleep(5)